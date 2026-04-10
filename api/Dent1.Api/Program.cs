using Dent1.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation()   // will have constroller , filters 
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationModules(builder.Configuration)  // registering sevices and infra 
    .AddApiServices()
    .AddCorsPolicies();

var app = builder.Build();

app.UsePresentationPipeline();

await app.SeedAsync();

app.Run();
