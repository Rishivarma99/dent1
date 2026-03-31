using Dent1.Business;
using Dent1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── CQRS Bootstrappers ──────────────────────────────────
// DataBootstrapper: registers DbContext with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
DataBootstrapper.Register(builder.Services, connectionString);

// BusinessBootstrapper: registers MediatR (auto-discovers all Command & Query handlers)
BusinessBootstrapper.Register(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
