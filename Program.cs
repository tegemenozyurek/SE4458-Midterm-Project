using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using API_Project;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configure CORS to allow all origins (use restrictive policy in production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 2. Configure API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version"));
});

// 3. Add Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SE4458 Midterm Project",
        Description = "This API manages short-term accommodations for hosts, guests, and admins. It allows listing properties, booking stays, adding reviews, and generating reports. Designed for mobile and web apps handling vacation rentals and short-term stays."
    });
});

// 4. Add DbContext with MySQL connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 32))));

// 5. Add HttpClient support for GatewayController
builder.Services.AddHttpClient();

// 6. Add MVC Controllers
builder.Services.AddControllers();

// 7. Add Endpoint API Explorer
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// 8. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 9. Enable CORS (this must come before `MapControllers`)
app.UseCors("AllowAllOrigins");

// 10. Enable HTTPS redirection
app.UseHttpsRedirection();

// 11. Add Routing and Controller Mapping
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
