using BlazorCrud.Interfaces;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using BlazorCrud.Shared;
using Microsoft.AspNetCore.Components;
using myActivityEvent = BlazorCrud.Providers.ActivityEvent;

namespace BlazorCrud.Providers
{
    public partial class JaegerTelemetryProvider : ITelemetryProvider, IDisposable
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ActivitySource ActivitySource { get; private set; } = new("Blazor");
        private readonly TracerProvider _tracerProvider = null!;
        public async Task ReceiveBundle(ActivityBundle activityBundle) {
            await Task.Run(() =>
            {
                using (var activity = ActivitySource.StartActivity(activityBundle.ActivityName))
                {
                    foreach (Providers.ActivityEvent events in (activityBundle.ActivityEvents))
                    {
                        activity?.AddEvent(new System.Diagnostics.ActivityEvent(events.EventName));
                        foreach (KeyValuePair<string, object> keyValuePair in events.EventTag)
                        {
                            activity?.SetTag(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                }
            });
        }
        public JaegerTelemetryProvider()
        {
            _tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Blazor"))
                .AddSource("Blazor")
                .ConfigureResource(resource => resource.AddService("Navigation"))
                .AddZipkinExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                    o.ExportProcessorType = ExportProcessorType.Simple;
                })
                .Build();
        }

        public void Dispose()
        {
            _tracerProvider.Dispose();
        }
    }
}

//activity?.SetTag(activityBundle.ActivityEvents[0].EventTag.Keys, activityBundle.ActivityEvents[0].EventTag.Values);
//activity?.SetTag("Number of times the home page is visited", HomeCtr);
//activity?.SetTag("Number of times the Counter page is visited", CounterPageCtr);
//activity?.SetTag("Number of times the Fetch data page is visited", FetchDataCtr);
//activity?.SetTag("Number of times the persons page is visited", PersonsCtr);
//activity?.SetTag("Number of times the add person page is visited", AddPersonCtr);
