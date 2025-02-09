namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class DoctorTimeSlotsRequest
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public int AppointmentTypeId { get; set; }
    }
}
