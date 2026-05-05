using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Root.API.Application.Abstractions;
using Root.API.Application.Common.Abstractions;
using Root.API.Application.Common.Behaviors;
using Root.API.Infrastructure.Persistence;
using Root.API.Infrastructure.Persistence.Seed;
using Root.API.Infrastructure.Security;
using Root.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Database ──────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? BuildConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseNpgsql(connectionString));

// ── MediatR + Pipeline Behaviors ──────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ── FluentValidation ──────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ── Infrastructure Services ───────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is required.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // FR-030: tokens never expire
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

// ── Authorization Policies ────────────────────────────────────────────────────
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    opts.AddPolicy("AdminOrAgent", policy => policy.RequireRole("admin", "agent"));
    opts.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Root API – User Role Management",
        Version = "v1"
    });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Bearer token. Format: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "bearerAuth"
        }
    };

    opts.AddSecurityDefinition("bearerAuth", bearerScheme);
    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Migrate and Seed ──────────────────────────────────────────────────────────
await MigrateAndSeedAsync(app);

app.Run();

// ── Helpers ───────────────────────────────────────────────────────────────────
static string BuildConnectionString(IConfiguration config)
{
    var host = config["PostgreSQL:Host"] ?? "localhost";
    var port = config["PostgreSQL:Port"] ?? "5432";
    var database = config["PostgreSQL:Database"] ?? "root_memory";
    var user = config["PostgreSQL:User"] ?? "user";
    var password = config["PostgreSQL:Password"] ?? "123456";
    return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
}

static async Task MigrateAndSeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Migrations applied successfully.");

        logger.LogInformation("Running database seed...");
        await DatabaseSeeder.SeedAsync(context, passwordHasher, logger);
        logger.LogInformation("Database seed completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration/seed.");
        throw;
    }
}
