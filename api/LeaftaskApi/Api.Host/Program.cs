using Api.Host.Infrastructure;
using Api.Host.Infrastructure.DatabaseExtensions;
using BuildingBlocks.DrivingInfrastructure;
using Modules.Users.DrivingInfrastructure.Setup;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// --- Host base configuration ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Error handling configuration ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- Register modules and building blocks ---
builder.Services.AddBuildingBlocks();
builder.Services.AddUsersModule(builder.Configuration, builder.Environment.IsDevelopment());

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

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
