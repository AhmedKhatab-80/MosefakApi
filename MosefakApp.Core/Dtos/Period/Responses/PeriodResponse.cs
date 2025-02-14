namespace MosefakApp.Core.Dtos.Period.Responses
{
    public class PeriodResponse
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int WorkingTimeId { get; set; } 
        public bool IsAvailable { get; set; }
    }
}
