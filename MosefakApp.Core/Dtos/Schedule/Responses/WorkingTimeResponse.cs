namespace MosefakApp.Core.Dtos.Schedule.Responses
{
    public class WorkingTimeResponse
    {
        public int Id { get; set; }
        public DayOfWeek Day { get; set; }
        public ICollection<PeriodResponse> Periods { get; set; } = new HashSet<PeriodResponse>();
        public int ClinicId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
