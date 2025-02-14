namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class PeriodConfiguration : IEntityTypeConfiguration<Period>
    {
        public void Configure(EntityTypeBuilder<Period> builder)
        {
            builder.ToTable("Periods").HasKey(x => x.Id);

            builder.Property(x => x.StartTime).HasColumnType("TIME").IsRequired();
            builder.Property(x => x.EndTime).HasColumnType("TIME").IsRequired();
        }
    }
}
