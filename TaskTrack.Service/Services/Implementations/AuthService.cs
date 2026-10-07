using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.DTOs.Requests;
using TaskTrack.Repo.DTOs.Responses;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Helpers;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Implementations;

public class AuthService : IAuthService
{
    private const short StaffRole = 0;
    private readonly IAccountRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthService(IAccountRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<AccountResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("Full name is required.");
        ValidatePasswordLength(request.Password);
        var email = NormalizeEmail(request.Email);
        if (await _repository.GetByEmailAsync(email) is not null)
            throw new ConflictException("An account with this email already exists.");

        var account = new SystemAccount
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = StaffRole,
            CreatedDate = DateTime.Now
        };

        try
        {
            await _repository.AddAsync(account);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ConflictException("An account with this email already exists.");
        }

        return ToResponse(account);
    }

    public async Task<AuthenticationResponse?> LoginAsync(LoginRequest request)
    {
        var account = await _repository.GetByEmailAsync(NormalizeEmail(request.Email));
        if (account is null || !BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
            return null;

        var secret = _configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(secret) || Encoding.UTF8.GetByteCount(secret) < 32)
            throw new InvalidOperationException("JWT signing key is not configured or is shorter than 32 bytes.");

        var now = DateTime.UtcNow;
        var expiresAt = now.AddHours(24);
        var claims = new[]
        {
            new Claim("AccountID", account.AccountId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, account.AccountId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, account.Email),
            new Claim("Role", account.Role.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthenticationResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            Account = ToResponse(account)
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static void ValidatePasswordLength(string password)
    {
        if (Encoding.UTF8.GetByteCount(password) > 72)
            throw new ArgumentException("Password must not exceed 72 UTF-8 bytes.");
    }

    internal static AccountResponse ToResponse(SystemAccount account) => new()
    {
        AccountId = account.AccountId,
        FullName = account.FullName,
        Email = account.Email,
        Role = account.Role,
        CreatedDate = account.CreatedDate
    };
}