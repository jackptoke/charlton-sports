using CharltonSport.API.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options => {
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    // Various options to specify API version
    //options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
    //options.ApiVersionReader = new QueryStringApiVersionReader("cs-api-version");
});

// To Fix Swagger Api Version
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShopContext>(options => options.UseInMemoryDatabase("Shop"));

// 1. Enabling CORS
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(builder =>
//    {
//        builder.WithOrigins("https://localhost:7257").WithHeaders("X-API-Version");
//    });
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Enforcing https only traffic
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 2. Enabling CORS
//app.UseCors();

app.MapControllers();

app.Run();
