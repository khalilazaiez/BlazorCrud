using System.Diagnostics;

namespace BlazorCrud.Interfaces
{
    public interface ITelemetryProvider
    {
        public ActivitySource ActivitySource { get; }
    }
}
