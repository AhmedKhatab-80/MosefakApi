namespace MosefakApp.Core.Dtos.AppointmentType.Requests
{
    public class AppointmentTypeRequest
    {
        public TimeOnly Duration { get; set; } // validate it's positive value
        public string VisitType { get; set; } = null!;
        public decimal ConsultationFee { get; set; } // must greater than or equal 0
    }
}
