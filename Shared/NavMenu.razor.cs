using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using OpenTelemetry.Resources;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Unity;



namespace BlazorCrud.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]

        public MainLayout main { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        private bool collapseNavMenu = true;
        private DateTimeOffset hoverStartTime;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private async Task OnHoverStart(MouseEventArgs args)
        {
            hoverStartTime = DateTimeOffset.UtcNow;
        }


        private async Task Trace()
        {
            await Task.Run(() =>
            {
                using var otel = Sdk.CreateTracerProviderBuilder()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("blazor"))
                    .AddSource("Blazor")
                    .ConfigureResource(resource => resource.AddService("BlazorHover"))
                    .AddZipkinExporter(o =>
                    {
                        o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                        o.ExportProcessorType = ExportProcessorType.Simple;
                    })
                    //.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console)
                    .Build();
                using var source = new ActivitySource("Blazor");
                using (var activity = source.StartActivity("Hover"))
                {
                    activity?.AddEvent(new ActivityEvent("Hover"));
                }
            });
        }

        


        private async Task OnHoverEnd(MouseEventArgs args)
        {
            var hoverDuration = DateTimeOffset.UtcNow - hoverStartTime;
            if (hoverDuration.TotalSeconds >= 3)
            {
                await Trace();
            }
        }
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }


    }
}
