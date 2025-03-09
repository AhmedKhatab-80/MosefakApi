namespace MosefakApp.Infrastructure.Data.EntitiesConfig
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews").HasKey(x => x.Id);            
        }
    }
}
