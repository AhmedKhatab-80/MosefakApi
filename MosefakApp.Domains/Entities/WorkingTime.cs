namespace MosefakApp.Domains.Entities
{
    public class WorkingTime : BaseEntity // that represent Schedule
    {
        public DayOfWeek Day { get; set; } 
        public ICollection<Period> Periods { get; set; } = new HashSet<Period>();
        public int ClinicId { get; set; }       
        public Clinic Clinic { get; set; } = null!;    

        [NotMapped]
        public bool IsAvailable => Periods.Any(p => p.IsAvailable);
    }
}
