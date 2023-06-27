using BlazorCrud.Providers;
using System.Diagnostics;

namespace BlazorCrud.Interfaces
{
    public interface ITelemetryProvider
    {
        public ActivitySource ActivitySource { get; }
        public Task ReceiveBundle(ActivityBundle activityBundle);
    }
    
}
