using Prometheus.Client;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace BlazorCrud.Data
{
    public class UserActivityService
    {

        private static readonly HashSet<string> ActiveUsers = new HashSet<string>();

        public const string ServiceName = "MyService";
        public static Meter Meter = new(ServiceName);
        public static Counter<long> RequestCounter =
        Meter.CreateCounter<long>("app.request_counter");
        
        public void TrackUserActivity(string userId)
        {
            lock (ActiveUsers)
            {
                ActiveUsers.Add(userId);
                IncrementActiveUserCount();
            }
        }

        public int GetActiveUserCount()
        {
            lock (ActiveUsers)
            {
                return ActiveUsers.Count;

            }
        }
        public void RemoveInactiveUser(string userId)
        {
            lock (ActiveUsers)
            {
                ActiveUsers.Remove(userId);
            }
        }
        public void IncrementActiveUserCount() {
            RequestCounter.Add(1);
        }
        public void DecrementActiveUserCount() {
            RequestCounter.Add(-1);
        }

    }
}
