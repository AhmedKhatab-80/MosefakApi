namespace MosefakApp.Infrastructure.Data.context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<WorkingTime> WorkingTimes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ClinicAddress> ClinicAddresses { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Payment> Payments { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseEntity>();

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            var CurrentUserIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            int? CurrentUserId = null;

            if (CurrentUserIdClaim != null && int.TryParse(CurrentUserIdClaim.Value, out var parsedUserId))
            {
                CurrentUserId = parsedUserId;
            }

            foreach (var entryEntity in entries)
            {
                if (entryEntity != null && CurrentUserId is not null)
                {

                    if (entryEntity.State == EntityState.Added)
                    {
                        entryEntity.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                        entryEntity.Property(x => x.CreatedByUserId).CurrentValue = CurrentUserId.Value;
                    }
                    else if (entryEntity.State == EntityState.Modified)
                    {

                        if (entryEntity.Property(x => x.FirstUpdatedTime).CurrentValue is null && entryEntity.Property(x => x.FirstUpdatedByUserId).CurrentValue is null)
                        {
                            entryEntity.Property(x => x.FirstUpdatedByUserId).CurrentValue = CurrentUserId.Value;
                            entryEntity.Property(x => x.FirstUpdatedTime).CurrentValue = DateTime.UtcNow;
                        }
                        else
                        {
                            entryEntity.Property(x => x.LastUpdatedByUserId).CurrentValue = CurrentUserId.Value;
                            entryEntity.Property(x => x.LastUpdatedTime).CurrentValue = DateTime.UtcNow;
                        }
                    }
                    else if (entryEntity.State == EntityState.Deleted && entryEntity.Entity is ISoftDeletable)
                    {
                        entryEntity.State = EntityState.Modified;

                        entryEntity.Property(x => x.DeletedTime).CurrentValue = DateTime.Now;
                        entryEntity.Property(x => x.IsDeleted).CurrentValue = true;
                        entryEntity.Property(x => x.DeletedByUserId).CurrentValue = CurrentUserId.Value;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
    //public class BloggingContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    //{
    //    public AppDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();


    //        optionsBuilder.UseSqlServer("server=.; database=MosefakApp; Integrated Security=SSPI; trustServerCertificate=true;");


    //        return new AppDbContext(optionsBuilder.Options);
    //    }
    //}
}
