namespace MosefakApp.Infrastructure.Identity.IdentityEntitiesConfig
{
    public class ApplicationUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.OwnsOne(x => x.Address);

            builder.OwnsMany(x => x.RefreshTokens);

            builder.Property(x => x.FirstName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(250).IsRequired();

        }
    }
}
