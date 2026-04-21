using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using ServiceMatch.API.Middleware;
using ServiceMatch.Application;
using ServiceMatch.Infrastructure;
using ServiceMatch.Infrastructure.Persistence;
using ServiceMatch.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ServiceMatch DK API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiter(opts =>
{
    opts.AddFixedWindowLimiter("auth", limiterOpts =>
    {
        limiterOpts.PermitLimit = 10;
        limiterOpts.Window = TimeSpan.FromMinutes(1);
        limiterOpts.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOpts.QueueLimit = 0;
    });
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? ["http://localhost:3000"])
            .AllowAnyHeader()
            .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.MapHealthChecks("/api/v1/health");

if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

app.Run();

public partial class Program;
