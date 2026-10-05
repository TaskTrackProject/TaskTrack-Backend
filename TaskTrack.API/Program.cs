using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskTrack.Repo.Data;
using TaskTrack.Repo.Repositories.Implementations;
using TaskTrack.Repo.Repositories.Interfaces;
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

// Service
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();

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
                "https://tasktrack-fe.onrender.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

// CORS phải đặt trước Authorization
app.UseCors("AngularUI");

app.UseAuthorization();

app.MapControllers();

app.Run();

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