namespace MosefakApp.Core.Dtos.Doctor.Requests
{
    public class DoctorSearchFilter
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Search must contain only letters and spaces.")]
        public string Name { get; set; } = null!;
    }
}
