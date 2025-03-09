namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class SpecializationConfig : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.ToTable("Specializations").HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasConversion(new EnumToStringConverter<Specialty>());

            builder.Property(x => x.Category)
                   .HasConversion(new EnumToStringConverter<SpecialtyCategory>());
        }
    }
}
