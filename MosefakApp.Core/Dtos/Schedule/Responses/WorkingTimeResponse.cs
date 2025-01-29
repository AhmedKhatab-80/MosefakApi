namespace MosefakApp.Core.Dtos.Schedule.Responses
{
    public class WorkingTimeResponse
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; } // 08:00
        public TimeOnly EndTime { get; set; }   // 23:00
    }
}
