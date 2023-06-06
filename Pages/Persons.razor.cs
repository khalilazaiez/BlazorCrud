using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OpenTelemetry.Resources;
using OpenTelemetry;
using System.Diagnostics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;


namespace BlazorCrud.Pages
{
    public partial class Persons
    {

        private List<Models.Person> persons = new();
        private int AddedUsers;
        public NavigationManager NavigationManager { get; set; }


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
                    activity?.AddEvent(new ActivityEvent("Button clicked"));
                    activity?.SetTag("currentUrl", NavigationManager.Uri);
                    activity?.SetTag("AddedUsers", AddedUsers);
                }
            });
        }
        private async Task Delete(Models.Person person)
        {
            bool confirmed = await jsRuntime.InvokeAsync<bool>("confirm", "Are you sure????");
            if (confirmed)
            {
                if (personService.Delete(person.Id))
                {
                    persons.Remove(person);
                }

            }
        }

        protected override void OnInitialized()
        {
            persons = personService.GetAll();
            base.OnInitialized();

        }
    }
}
