namespace BlazorCrud.Providers
{
    public class ActivityBundle
    {
        public string ActivityName { get; set; }
        public List<ActivityEvent> ActivityEvents { get; set; }

        public ActivityBundle(string activityName, List<ActivityEvent> activityEvents)
        {
            ActivityName = activityName;
            ActivityEvents = activityEvents;
        }


    }
}
