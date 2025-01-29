namespace MosefakApp.Domains.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Gender? Gender { get; set; }
        public Address Address { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string? ImagePath { get; set; } // when register will not ask him to enter image, but in profile settings can upload image
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public bool IsDisabled { get; set; } = false;
        
        [NotMapped]
        public int Age 
        {
            get
            {
                if (DateOfBirth is null)
                    return 0;
                return DateTime.Now.Year - DateOfBirth.Value.Year;
            }
        }
    }
}
