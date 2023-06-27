using System.Diagnostics;
using BlazorCrud.Interfaces;
using BlazorCrud.Providers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Unity;

namespace BlazorCrud.Shared;

public partial class MainLayout
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    public ITelemetryProvider TelemetryProvider { get; set; }
    
    
    private  int HomeCtr = 0, CounterPageCtr = 0, FetchDataCtr = 0, PersonsCtr = 0, AddPersonCtr = 0;
    private  string? _currentUrl;
    private  DateTime StartTime = DateTime.UtcNow;
    
    //private static List<Providers.ActivityEvent> activityEvents = new List<Providers.ActivityEvent> { ActivityEvent };

   
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            NavigationManager.LocationChanged += async (sender, args) => await LocationChangedHandlerAsync(sender, args);
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    private async Task LocationChangedHandlerAsync(object sender, LocationChangedEventArgs args)
    {
        if (_currentUrl != Path.GetFileName(args.Location))
        {
            double textDuration = (DateTime.UtcNow - StartTime).TotalSeconds;
            Dictionary<string, object> EventTags = new Dictionary<string, object>()
            {
                {"Number of times the home page is visited", HomeCtr},
                {"Number of times the Counter page is visited", CounterPageCtr },
                {"Number of times the Fetch data page is visited", FetchDataCtr},
                {"Number of times the persons page is visited", PersonsCtr },
                {"Number of times the add person page is visited", AddPersonCtr },
                {$"Time spent on the {Path.GetFileName(_currentUrl)} page", textDuration }
            };
            Providers.ActivityEvent ActivityEvent = new Providers.ActivityEvent("Navigation location changed", EventTags);
            List<Providers.ActivityEvent> activityEvents = new List<Providers.ActivityEvent> { ActivityEvent };
            ActivityBundle ActivityBundle  = new ActivityBundle("Navigating", activityEvents);
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
            StartTime = DateTime.UtcNow;
            await TelemetryProvider.ReceiveBundle(ActivityBundle);
        }
        _currentUrl = Path.GetFileName(args.Location);
    }
}