namespace MosefakApp.Domains.Entities
{
    public class Specialization : BaseEntity // Note, SpecialtyCategory will need in Method SortBy All,General,Heart,etc...
    {
        public Specialty Name { get; set; } // Cardiology طب القلب
        public SpecialtyCategory Category { get; set; } // Heart القلب
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
    }
}
