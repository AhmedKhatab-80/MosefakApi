namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments").HasKey(x => new { x.Id, x.PatientId, x.DoctorId });

            builder.Property(x => x.AppointmentStatus)
                .HasConversion(new EnumToStringConverter<AppointmentStatus>());

            builder.Property(x => x.PaymentStatus)
                .HasConversion(new EnumToStringConverter<PaymentStatus>());

            builder.HasIndex(x => new { x.DoctorId, x.StartDate, x.EndDate }).IsUnique();

        }
    }
}
