namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class AppointmentTypeResponse
    {
        public string Id { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public string VisitType { get; set; } = null!;
        public decimal ConsultationFee { get; set; }
    }
}
