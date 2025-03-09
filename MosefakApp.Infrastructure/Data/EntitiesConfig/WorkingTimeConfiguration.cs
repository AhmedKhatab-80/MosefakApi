namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class WorkingTimeConfiguration : IEntityTypeConfiguration<WorkingTime>
    {
        public void Configure(EntityTypeBuilder<WorkingTime> builder)
        {
            builder.ToTable("WorkingTimes").HasKey(x => x.Id);

            builder.Property(x=> x.Day)
                .HasConversion(new EnumToStringConverter<DayOfWeek>());

        }
    }
}
