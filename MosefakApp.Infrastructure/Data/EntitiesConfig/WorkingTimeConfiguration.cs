namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class WorkingTimeConfiguration : IEntityTypeConfiguration<WorkingTime>
    {
        public void Configure(EntityTypeBuilder<WorkingTime> builder)
        {
            builder.ToTable("WorkingTimes").HasKey(x => x.Id);

            builder.Property(x=> x.DayOfWeek).HasConversion(
                v=> v.ToString(),
                v => (DayOfWeek)Enum.Parse(typeof(DayOfWeek), v));

            builder.Property(x => x.StartTime).HasColumnType("TIME").IsRequired();
            builder.Property(x => x.EndTime).HasColumnType("TIME").IsRequired();
        }
    }
}
