namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments").HasKey(x => x.Id);

            builder.Property(x=> x.Amount).HasColumnType("decimal").HasPrecision(18,2).IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion(new EnumToStringConverter<PaymentStatus>());
        }
    }
}
