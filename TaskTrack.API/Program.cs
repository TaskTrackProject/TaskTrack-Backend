using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories.Implementations;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.API.Middleware;
using TaskTrack.Service.Helpers;
using TaskTrack.Service.Implementations;
using TaskTrack.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrWhiteSpace(databaseUrl))
    connectionString = ToNpgsqlConnectionString(databaseUrl);

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection or DATABASE_URL.");

builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Repository
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Service
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Validator
builder.Services.AddScoped<ProjectValidator>();
builder.Services.AddScoped<TaskValidator>();
builder.Services.AddScoped<TagValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularUI", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://tasktrack-fe.onrender.com",
                "https://task-track-frontend-olive.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
    throw new InvalidOperationException("Configure Jwt:Secret with a key of at least 32 bytes using User Secrets or an environment variable.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidIssuer = jwtIssuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "Email",
            RoleClaimType = "Role"
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT access token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

await SeedInitialAdminAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// CORS phải đặt trước Authorization
app.UseCors("AngularUI");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async System.Threading.Tasks.Task SeedInitialAdminAsync(IServiceProvider services, IConfiguration configuration)
{
    var fullName = configuration["InitialAdmin:FullName"];
    var email = configuration["InitialAdmin:Email"];
    var password = configuration["InitialAdmin:Password"];
    if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
        return;
    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        throw new InvalidOperationException("Set InitialAdmin:FullName, InitialAdmin:Email, and InitialAdmin:Password together.");
    if (Encoding.UTF8.GetByteCount(password) < 8 || Encoding.UTF8.GetByteCount(password) > 72)
        throw new InvalidOperationException("Initial admin password must be between 8 and 72 UTF-8 bytes.");

    await using var scope = services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
    var normalizedEmail = email.Trim().ToLowerInvariant();
    if (await context.SystemAccounts.AnyAsync(account => account.Email == normalizedEmail))
        return;

    context.SystemAccounts.Add(new SystemAccount
    {
        FullName = fullName.Trim(),
        Email = normalizedEmail,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        Role = 1,
        CreatedDate = DateTime.Now
    });
    await context.SaveChangesAsync();
}

static string ToNpgsqlConnectionString(string url)
{
    if (!url.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
        !url.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        return url;

    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':', 2);

    return new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port > 0 ? uri.Port : 5432,
        Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
        Username = Uri.UnescapeDataString(userInfo[0]),
        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
        SslMode = SslMode.Prefer
    }.ToString();
}