namespace BlazorCrud.Providers
{
    public class ActivityEvent
    {
        public string EventName { get; set; }
        public Dictionary<string, object> EventTag { get; set; }
        public ActivityEvent(string eventName, Dictionary<string, object> eventTag)
        {
            EventName = eventName;
            EventTag = eventTag;
        }
    }
}
