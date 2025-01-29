namespace MosefakApp.Domains.Entities
{
    public class WorkingTime : BaseEntity // that represent Schedule
    {
        public int DoctorId { get; set; } 
        public Doctor Doctor { get; set; } = null!;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; } // 08:00
        public TimeOnly EndTime { get; set; }   // 23:00
    }
}
