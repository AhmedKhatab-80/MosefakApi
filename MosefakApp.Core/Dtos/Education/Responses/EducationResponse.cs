namespace MosefakApp.Core.Dtos.Education.Responses
{
    public class EducationResponse
    {
        public int Id { get; set; }
        public string Degree { get; set; } = null!; // e.g., MBBS, MD, PhD
        public string Major { get; set; } = null!; // e.g., Cardiology, Surgery, Pediatrics
        public string UniversityName { get; set; } = null!; // e.g., Harvard Medical School
        public string? UniversityLogoPath { get; set; }  // Optional: Store URL/Path to University Logo
        public string Location { get; set; } = null!; // e.g., Boston, USA
        public DateOnly StartDate { get; set; } // Start Year of the Degree
        public DateOnly? EndDate { get; set; } // End Year (Graduation Year)
        public bool CurrentlyStudying { get; set; } // If still studying (no EndDate)
        public string? AdditionalNotes { get; set; } // Optional field for Thesis, Research, etc.
        public int DoctorId { get; set; }
    }
}
