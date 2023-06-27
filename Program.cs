using BlazorCrud.Data;
using BlazorCrud.Models;
using BlazorCrud.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using BlazorCrud.Interfaces;
using BlazorCrud.Providers;
using System.Diagnostics.Metrics;
using OpenTelemetry.Exporter;
using Prometheus;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddScoped<ITelemetryProvider, JaegerTelemetryProvider>();
        builder.Services.AddScoped<IMetricsProvider, PrometheusTelemetryProvider>();
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddTransient<IPersonService, PersonService>();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromSeconds(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddSingleton<UserActivityService>();
        builder.Services.AddSingleton<UserMetricsServices>();

        

        builder.Services.AddOpenTelemetry().WithTracing(
            builder => builder.AddAspNetCoreInstrumentation());


        //addscope
        builder.Services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddConsoleExporter()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter(UserActivityService.Meter.Name)
                .AddPrometheusExporter());


         builder.Services.AddDbContext<DatabaseContext>(options =>
            options.UseInMemoryDatabase("bd"));


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        app.UseMiddleware<UserActivityMiddleware>();
        app.UseStaticFiles();
        app.UseHttpsRedirection();



        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}

public static class DiagnosticsConfig
{
    public const string ServiceName = "MyService";
    public static ActivitySource ActivitySource = new ActivitySource(ServiceName);
}
