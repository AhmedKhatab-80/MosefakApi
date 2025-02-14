namespace MosefakApp.Core.Dtos.Schedule.Requests
{
    public class WorkingTimeRequest
    {
        public DayOfWeek Day { get; set; }
        public ICollection<PeriodRequest> Periods { get; set; } = new HashSet<PeriodRequest>();
    }
}
