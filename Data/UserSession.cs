namespace BlazorCrud.Data
{
    public class UserSession
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan SessionTime => EndTime - StartTime;
    }
}
