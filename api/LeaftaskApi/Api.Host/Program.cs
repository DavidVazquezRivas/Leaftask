using Api.Host;
using Api.Host.Infrastructure;
using Api.Host.Infrastructure.DatabaseExtensions;
using BuildingBlocks.DrivingInfrastructure;
using BuildingBlocks.DrivingInfrastructure.Jobs.Quartz;
using Modules.Organizations.DrivingInfrastructure.Setup;
using Modules.Users.DrivingInfrastructure.Setup;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext());

// --- Host base configuration ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCorsConfiguration(builder.Configuration);

// --- Error handling configuration ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Register modules and building blocks ---
builder.Services.AddBuildingBlocks();
builder.Services.AddQuartzInfrastructure();
builder.Services.AddUsersModule(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddOrganizationsModule(builder.Configuration, builder.Environment.IsDevelopment());

// --- Authentication and authorization configuration ---
builder.Services.AddAuthenticationConfig(builder.Configuration);
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// --- Database migration and seeding ---
await app.ApplyAllMigrationsAsync();
await app.SeedAllDataAsync();

// --- HTTP pipeline configuration ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
