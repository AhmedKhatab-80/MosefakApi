namespace MosefakApp.Infrastructure.Identity.IdentityEntitiesConfig
{
    public class AppUserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.OwnsOne(x => x.Address);

            builder.Property(x => x.FirstName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(250).IsRequired();

            builder.Property(x => x.Gender).HasConversion(
                x => x.ToString(),
                x => (Gender)Enum.Parse(typeof(Gender), x!)
                );

        }
    }
}
