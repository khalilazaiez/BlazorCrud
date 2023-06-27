using System.Threading.Tasks;
using BlazorCrud.Data;
using Microsoft.AspNetCore.Http;

namespace BlazorCrud.Providers
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserActivityService _activityService;

        public UserActivityMiddleware(RequestDelegate next, UserActivityService activityService)
        {
            _next = next;
            _activityService = activityService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Retrieve or create a unique user identifier (e.g., user ID, session ID)
            string userId = context.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                userId = GenerateUniqueUserId();
                context.Session.SetString("UserId", userId);
            }

            // Track user activity
            _activityService.TrackUserActivity(userId);

            // Process the request
            await _next(context);

            // Remove inactive user after the response is complete
            _activityService.RemoveInactiveUser(userId);
        }

        private string GenerateUniqueUserId()
        {
            // Implement your logic to generate a unique user identifier
            // For simplicity, you can use Guid.NewGuid().ToString() or any other method
            return Guid.NewGuid().ToString();
        }
    }
}
