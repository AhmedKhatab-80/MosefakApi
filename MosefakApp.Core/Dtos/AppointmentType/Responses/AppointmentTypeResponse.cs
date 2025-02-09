namespace MosefakApp.Core.Dtos.AppointmentType.Responses
{
    public class AppointmentTypeResponse
    {
        public int Id { get; set; }
        public TimeOnly Duration { get; set; }
        public string VisitType { get; set; } = null!;
        public decimal ConsultationFee { get; set; }
    }
}
