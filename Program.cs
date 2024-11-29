using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using API_Project;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    // Set the default version to 1.0
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // Optionally, specify a version reader (like from the URL or headers)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-api-version"),    // Check API version in header
        new QueryStringApiVersionReader("api-version")  // Or check API version in query string
    );
});

// Add Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SE4458 Midterm Project",
        Description = "This API manages short-term accommodations for hosts, guests, and admins. It allows listing properties, booking stays, adding reviews, and generating reports. Designed for mobile and web apps handling vacation rentals and short-term stays."
    });
});

// Add DbContext with MySQL connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))
    ));

// Add other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
