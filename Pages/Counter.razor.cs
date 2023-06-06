using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Components;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BlazorCrud.Pages;

public partial class Counter
{
    private Meter Meter { get; set; }
    private int currentCount;
    private Counter<int> MyCounter;
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private async Task Trace()
    {
        await Task.Run(() =>
        {
            using var otel = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("blazor"))
                .AddSource("Blazor")
                .ConfigureResource(resource => resource.AddService("BlazorClick"))
                .AddZipkinExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                    o.ExportProcessorType = ExportProcessorType.Simple;
                })
                //.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console)
                .Build();
            Console.WriteLine("Test.");
            using var source = new ActivitySource("Blazor");
            using (var activity = source.StartActivity("Click"))
            {
                activity?.AddEvent(new ActivityEvent("Button clicked"));
                activity?.SetTag("currentUrl", NavigationManager.Uri);
                activity?.SetTag("count", currentCount);
            }
        });
    }

    private async Task IncrementCountAsync()
    {
        currentCount++;
        await Trace();
    }

    public Counter()
    {
        Meter = new Meter(DiagnosticsConfig.ServiceName);
        MyCounter = Meter.CreateCounter<int>("clicks");
    }
}