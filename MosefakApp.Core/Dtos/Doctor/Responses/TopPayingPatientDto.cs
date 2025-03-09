namespace MosefakApp.Core.Dtos.Doctor.Responses
{
    public class TopPayingPatientDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
    }
}
