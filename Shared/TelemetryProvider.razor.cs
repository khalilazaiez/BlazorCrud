using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Unity;

namespace BlazorCrud.Shared
{
    public partial class TelemetryProvider
    {
        public NavigationManager NavigationManager { get; set; }
        private int HomeCtr = 0, CounterPageCtr = 0, FetchDataCtr = 0, PersonsCtr = 0, AddPersonCtr = 0;
        private string? _currentUrl;
        private DateTime StartTime = DateTime.UtcNow;

        public async Task LogEventAsync(ActivitySource source) {
            DateTimeOffset TimeSpent;
            using var otel = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("blazor"))
                .AddSource("Blazor")
                .ConfigureResource(resource => resource.AddService("Navigation"))
                .AddZipkinExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                    o.ExportProcessorType = ExportProcessorType.Simple;
                })
                .AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console)
                .Build();
            using (var activity = source.StartActivity("Navigating"))
            {
                var textDuration = (DateTime.UtcNow - StartTime).TotalSeconds;
                activity?.AddEvent(new ActivityEvent("Navigation location changed"));
                activity?.SetTag($"Time spent on the {Path.GetFileName(_currentUrl)} page", textDuration);

                if (NavigationManager.Uri == "http://localhost:5075/")
                    HomeCtr++;
                else if (NavigationManager.Uri == "http://localhost:5075/counter")
                    CounterPageCtr++;
                else if (NavigationManager.Uri == "http://localhost:5075/fetchdata")
                    FetchDataCtr++;
                else if (NavigationManager.Uri == "http://localhost:5075/persons")
                    PersonsCtr++;
                else if (NavigationManager.Uri == "http://localhost:5075/person/add")
                    AddPersonCtr++;
                activity?.SetTag("Number of times the home page is visited", HomeCtr);
                activity?.SetTag("Number of times the Counter page is visited", CounterPageCtr);
                activity?.SetTag("Number of times the Fetch data page is visited", FetchDataCtr);
                activity?.SetTag("Number of times the persons page is visited", PersonsCtr);
                activity?.SetTag("Number of times the add person page is visited", AddPersonCtr);
                StartTime = DateTime.UtcNow;
            }
        }
    }
}