using BlazorCrud.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Unity;

namespace BlazorCrud.Pages
{
    public partial class Index
    {
        [Inject]
        public UserActivityService ActivityService { get; set; }

        [Inject]
        public UserMetricsServices MetricsService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Start a session for the current user (you can replace "userId" with the actual user ID)
                MetricsService.StartSession("userId");

                // Simulate some time passing (you can remove this code or replace it with your actual logic)
                await Task.Delay(5000);

                // End the session for the current user
                MetricsService.EndSession("userId");

                // Force the component to re-render after session tracking is complete
                StateHasChanged();
            }
        }
    }
}
