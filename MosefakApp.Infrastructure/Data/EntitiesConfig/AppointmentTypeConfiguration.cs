namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class AppointmentTypeConfiguration : IEntityTypeConfiguration<AppointmentType>
    {
        public void Configure(EntityTypeBuilder<AppointmentType> builder)
        {
            builder.ToTable("AppointmentTypes").HasKey(x => x.Id);

        }
    }
}
