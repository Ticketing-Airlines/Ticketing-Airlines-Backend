using Airline1.Data;
using Airline1.Mappings;
using Airline1.Repositories.Implementations;
using Airline1.Repositories.Interfaces;
using Airline1.Services.Implementations;
using Airline1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers()
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
.AddJsonOptions(opts =>
 {
     // preserve property names as defined (PascalCase) if you prefer:
     opts.JsonSerializerOptions.PropertyNamingPolicy = null;
 });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Airline1 API", Version = "v1" });
});

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBMS")));

// AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddMaps(typeof(MappingProfile).Assembly);
});

// DI registrations
builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAirportService, AirportService>();

builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();
builder.Services.AddScoped<IAircraftService, AircraftService>();

builder.Services.AddScoped<IFlightRouteRepository, FlightRouteRepository>();
builder.Services.AddScoped<IFlightRouteService, FlightRouteService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())

    // middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airline1 API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
