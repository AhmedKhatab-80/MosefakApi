namespace MosefakApp.Core.Dtos.Schedule.Requests
{
    public class WorkingTimeRequest
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; } // 08:00
        public TimeOnly EndTime { get; set; }   // 23:00
    }
}
