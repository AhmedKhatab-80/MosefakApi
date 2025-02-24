
using Microsoft.EntityFrameworkCore.Query;

namespace MosefakApp.Infrastructure.Data.context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<WorkingTime> WorkingTimes { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseEntity>();


            // Configure global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertFilterExpression<ISoftDeletable>(e => !e.IsDeleted, entityType.ClrType));
                }
            }

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                                               .SelectMany(t => t.GetForeignKeys())
                                               .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        private static LambdaExpression ConvertFilterExpression<TInterface>(
            Expression<Func<TInterface, bool>> filterExpression, Type entityType)
        {
            var parameter = Expression.Parameter(entityType);
            var body = ReplacingExpressionVisitor.Replace(
                filterExpression.Parameters[0],
                parameter,
                filterExpression.Body);
            return Expression.Lambda(body, parameter);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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

                        // ✅ Check if it's a new Review, then increment the Doctor's NumberOfReviews
                        if (entryEntity.Entity is Review review)
                        {
                            await UpdateDoctorReviewCount(review.AppUserId, 1);
                        }
                    }
                    else if (entryEntity.State == EntityState.Modified)
                    {
                        if (entryEntity.Properties.Any(p => p.IsModified)) //  Only update if properties are modified
                        {
                            if (entryEntity.Property(x => x.FirstUpdatedTime).CurrentValue is null &&
                            entryEntity.Property(x => x.FirstUpdatedByUserId).CurrentValue is null)
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
                    }
                    else if (entryEntity.State == EntityState.Deleted && entryEntity.Entity is ISoftDeletable)
                    {
                        entryEntity.State = EntityState.Modified;
                        entryEntity.Entity.MarkAsDeleted(CurrentUserId.Value);

                        // ✅ Check if a Review is being deleted, then decrement the Doctor's NumberOfReviews
                        if (entryEntity.Entity is Review review)
                        {
                            await UpdateDoctorReviewCount(review.AppUserId, -1);
                        }
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateDoctorReviewCount(int doctorId, int change)
        {
            var doctor = await Set<Doctor>().FirstOrDefaultAsync(d => d.Id == doctorId);
            if (doctor is not null)
            {
                doctor.NumberOfReviews += change;
                await SaveChangesAsync(); // Ensure the update is persisted
            }
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
