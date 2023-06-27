using OpenTelemetry.Resources;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using BlazorCrud.Interfaces;

namespace BlazorCrud.Providers
{
    public class PrometheusTelemetryProvider: IMetricsProvider
    {
        public const string MyAppSource = "Demo.AspNet";
        public static readonly Meter DemoMeter = new Meter(MyAppSource);
        public PrometheusTelemetryProvider()
        {
                 
        }
    }
}
