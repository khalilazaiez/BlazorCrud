using BlazorCrud.Interfaces;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry;
using System.Diagnostics;
using BlazorCrud.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorCrud.Providers
{
    public class ConsoleTelemetryProvider : ITelemetryProvider
    {
        public ActivitySource ActivitySource { get; private set; } = new("Blazor");
        private readonly TracerProvider _tracerProvider = null!;

        public ConsoleTelemetryProvider()
        {
            _tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Blazor"))
                .AddSource("Blazor")
                .AddConsoleExporter()
                .Build();
        }

        //public async Task LogActivity(Dictionary<string, Dictionary<string, object>> activities)
        //{
        //    foreach(var activity in activities)
        //    {
        //        using(var activityEvent = ActivitySource.CreateActivity())
        //    }
        //}

        public void Dispose()
        {
            _tracerProvider.Dispose();
        }

        public async Task ReceiveBundle(ActivityBundle activityBundle)
        {
            throw new NotImplementedException();
        }
    }
}
