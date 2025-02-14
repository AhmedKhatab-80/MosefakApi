namespace MosefakApp.Core.Dtos.AppointmentType.Responses
{
    public class AppointmentTypeResponse
    {
        public int Id { get; set; }
        public TimeSpan Duration { get; set; }
        public string VisitType { get; set; } = null!;
        public decimal ConsultationFee { get; set; }
        public int DoctorId { get; set; }
    }
}
