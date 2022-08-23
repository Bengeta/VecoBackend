using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using VecoBackend.Data;
using VecoBackend.Interfaces;
using VecoBackend.Models;
using VecoBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ImageService>();
builder.Services.AddTransient<IImageProfile, BoxImageProfileModel>();
builder.Services.AddTransient<IImageProfile, LogoImageProfileModel>();

var connection = builder.Configuration.GetConnectionString("MainDB");
    //builder.Services.AddScoped<IMigratorService, MigratorService>();
builder.Services.AddDbContext<ApplicationContext>(x => x.UseNpgsql(connection));


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();