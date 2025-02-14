namespace MosefakApp.Domains.Entities
{
    public class Period : BaseEntity
    {
        public TimeOnly StartTime { get; set; }  
        public TimeOnly EndTime { get; set; }

        public int WorkingTimeId { get; set; } // FK
        public WorkingTime WorkingTime { get; set; } = null!; // Nav

        [NotMapped]
        public bool IsAvailable
        {
            get
            {
                var now = TimeOnly.FromDateTime(DateTime.Now);
                if (StartTime < EndTime)
                    return now >= StartTime && now <= EndTime;
                else // Handle overnight periods (e.g., 10 PM to 2 AM)
                    return now >= StartTime || now <= EndTime;
            }
        }
    }
}
