namespace MosefakApp.Core.Dtos.Schedule.Requests
{
    public class WorkingTimeRequest
    {
        public DayOfWeek Day { get; set; }
        public List<PeriodRequest> Periods { get; set; } = new List<PeriodRequest>();
    }
}
