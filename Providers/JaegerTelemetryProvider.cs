using BlazorCrud.Interfaces;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BlazorCrud.Providers
{
    public partial class JaegerTelemetryProvider : ITelemetryProvider, IDisposable
    {
        public ActivitySource ActivitySource { get; private set; } = new("Blazor");
        private readonly TracerProvider _tracerProvider = null!;

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
