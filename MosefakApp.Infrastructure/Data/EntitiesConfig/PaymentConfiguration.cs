namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments").HasKey(x => x.Id);

            builder.Property(x => x.PaymentMethod).HasConversion(
                x => x.ToString(),
                x => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), x)
                );
        }
    }
}
