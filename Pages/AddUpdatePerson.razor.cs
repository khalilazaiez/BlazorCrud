using Microsoft.AspNetCore.Components;
using OpenTelemetry.Resources;
using OpenTelemetry;
using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Unity;
using Microsoft.JSInterop;
using Radzen.Blazor.Rendering;

namespace BlazorCrud.Pages
{
    public partial class AddUpdatePerson
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Parameter]
        public int UserId { get; set; }
        private string message = string.Empty;
        private int AddedUsers;
        private DateTimeOffset textStartTime;
        public NavigationManager NavigationManager { get; set; }
        Models.Person person = new();
        private string Title = "Add Person";

        private async Task Save()
        {
            message = "wait...";
            if (personService.AddUpdate(person))
            {
                message = "Sucessfully added";
                person = new();
                await Trace();
            }
            else
            {
                message = "Could not saved";
            }
        }

        private async Task OnTextInput(ChangeEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(args.Value.ToString()))
            {
                var textDuration = DateTimeOffset.UtcNow - textStartTime;
                if (textDuration.TotalSeconds >= 5)
                {
                    await TraceText("tst");
                }
            }

        }
        private void OnfocusInput() {
            textStartTime = DateTimeOffset.UtcNow;
        }
        protected override void OnInitialized()
        {
            if (UserId > 0)
            {
                Title = "Upage Person";
                person = personService.FindById(UserId);
            }
            base.OnInitialized(); 

        }

        private async Task Trace()
        {
            await Task.Run(() =>
            {
                using var otel = Sdk.CreateTracerProviderBuilder()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("blazor"))
                    .AddSource("Blazor")
                    .ConfigureResource(resource => resource.AddService("AddPerson"))
                    .AddZipkinExporter(o =>
                    {
                        o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                        o.ExportProcessorType = ExportProcessorType.Simple;
                    })
                    //.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console)
                    .Build();
                using var source = new ActivitySource("Blazor");
                using (var activity = source.StartActivity("Click"))
                {
                    Console.WriteLine("Executing trace");
                    activity?.AddEvent(new ActivityEvent("Button clicked"));
                    activity?.SetTag("AddedUsers", AddedUsers);
                }
            });
        }

        private async Task TraceText(string typedText)
        {
            await Task.Run(() =>
            {
                using var otel = Sdk.CreateTracerProviderBuilder()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("blazor"))
                    .AddSource("Blazor")
                    .ConfigureResource(resource => resource.AddService("Input"))
                    .AddZipkinExporter(o =>
                    {
                        o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
                        o.ExportProcessorType = ExportProcessorType.Simple;
                    })
                    //.AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console)
                    .Build();
                using var source = new ActivitySource("Blazor");
                using (var activity = source.StartActivity("Type"))
                {
                    activity?.AddEvent(new ActivityEvent("Input deleted after 5 seconds"));
                    activity?.SetTag("TypedText", typedText);
                    Console.WriteLine("Executing trace text");
                }
            });
        }
    }
}
