namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments").HasKey(x => new { x.Id, x.AppUserId, x.DoctorId });

            builder.Property(x => x.AppointmentStatus).HasConversion(
                x => x.ToString(),
                x => (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), x)
                );

            builder.HasIndex(x => new { x.DoctorId, x.StartDate, x.EndDate }).IsUnique();
        }
    }
}
