using System.Text;
using Dent1.Api.Filters;
using Dent1.Api.Middleware;
using Dent1.Api.Services;
using Dent1.Business;
using Dent1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "FrontendCors";

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserSeedService, UserSeedService>();

// ── CQRS Bootstrappers ──────────────────────────────────
// DataBootstrapper: registers DbContext with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
DataBootstrapper.Register(builder.Services, connectionString);

// BusinessBootstrapper: registers MediatR (auto-discovers all Command & Query handlers)
BusinessBootstrapper.Register(builder.Services);

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var userSeedService = scope.ServiceProvider.GetRequiredService<IUserSeedService>();
    await userSeedService.SeedAsync(CancellationToken.None);
}

app.Run();
