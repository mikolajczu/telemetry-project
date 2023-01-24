using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mongo.AspNetCore.Identity;
using MongoDB.Driver;
using Telemetry.Entities;
using Telemetry.Entities.Models;
using Telemetry.Services.Mappers;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MongoDb");

Console.WriteLine($"Connection string to: {connectionString}");

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

builder.Services.AddIdentityWithMongoStoresUsingCustomTypes<User, MongoIdentityRole>(
        connectionString)
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:7256", "http://localhost:5196")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddTransient<ITelemetrySessionMapper, TelemetrySessionMapper>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();