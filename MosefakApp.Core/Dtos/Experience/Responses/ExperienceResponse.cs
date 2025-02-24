namespace MosefakApp.Core.Dtos.Experience.Responses
{
    public class ExperienceResponse
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? HospitalLogo { get; set; } 
        public string HospitalName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public EmploymentType EmploymentType { get; set; }
        public string? JobDescription { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; } 
        public bool CurrentlyWorkingHere { get; set; }
    }
}
