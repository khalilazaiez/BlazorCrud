using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorCrud.Data
{
    public class UserMetricsServices
    {
        private Dictionary<string, UserSession> activeSessions = new Dictionary<string, UserSession>();
        private List<UserSession> completedSessions = new List<UserSession>();

        public int CompletedSessionsCount => completedSessions.Count;
        public int ActiveSessionCount => activeSessions.Count;
        public TimeSpan AverageSessionTime => GetAverageSessionTime();

        public void StartSession(string userId)
        {
            if (!activeSessions.ContainsKey(userId))
            {
                var session = new UserSession
                {
                    StartTime = DateTime.Now,
                    EndTime = DateTime.MinValue
                };
                activeSessions.Add(userId, session);
            }
        }

        public void EndSession(string userId)
        {
            if (activeSessions.TryGetValue(userId, out var session))
            {
                session.EndTime = DateTime.Now;
                completedSessions.Add(session);
                activeSessions.Remove(userId);
            }
        }

        private TimeSpan GetAverageSessionTime()
        {
            if (completedSessions.Count > 0)
            {
                TimeSpan totalSessionTime = TimeSpan.Zero;
                foreach (var session in completedSessions)
                {
                    totalSessionTime += session.SessionTime;
                }

                return TimeSpan.FromTicks(totalSessionTime.Ticks / completedSessions.Count);
            }

            return TimeSpan.Zero;
        }
    }
}
