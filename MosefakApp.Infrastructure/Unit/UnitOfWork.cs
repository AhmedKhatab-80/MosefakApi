using Microsoft.Extensions.Configuration;

namespace MosefakApp.Infrastructure.Unit
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;

        private IDbContextTransaction _transaction;
        public IGenericRepositoryAsync<T> RepositoryAsync { get; }

        public IDoctorRepositoryAsync DoctorRepositoryAsync { get; }

        public IPatientRepositoryAsync PatientRepositoryAsync { get; }

        public IAppointmentRepositoryAsync AppointmentRepositoryAsync { get; }
        private readonly IConfiguration _configuration;
        public UnitOfWork(AppDbContext appDbContext, AppIdentityDbContext appIdentityDb, IConfiguration configuration)
        {
            _context = appDbContext;
            _configuration = configuration;
            RepositoryAsync = new GenericRepositoryAsync<T>(appDbContext);
            DoctorRepositoryAsync = new DoctorRepositoryAsync(appDbContext, appIdentityDb, _configuration);
            PatientRepositoryAsync = new PatientRepositoryAsync(appDbContext);
            AppointmentRepositoryAsync = new AppointmentRepositoryAsync(appDbContext);
        }


        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _transaction = await _context.Database.BeginTransactionAsync();

            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction in progress.");
            }
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

    }
}
