namespace MosefakApp.Core.Dtos.Period.Responses
{
    public class PeriodResponse
    {
        public string Id { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
