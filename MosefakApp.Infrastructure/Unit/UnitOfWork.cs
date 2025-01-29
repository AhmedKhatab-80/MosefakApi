namespace MosefakApp.Infrastructure.Unit
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : BaseEntity
    {
        private readonly AppDbContext _appDbContext;
        public IGenericRepositoryAsync<T> RepositoryAsync { get; }

        public IDoctorRepositoryAsync DoctorRepositoryAsync { get; }

        public IPatientRepositoryAsync PatientRepositoryAsync { get; }

        public IAppointmentRepositoryAsync AppointmentRepositoryAsync { get; }

        public UnitOfWork(AppDbContext appDbContext, AppIdentityDbContext appIdentityDb)
        {
            _appDbContext = appDbContext;
            RepositoryAsync = new GenericRepositoryAsync<T>(appDbContext);
            DoctorRepositoryAsync = new DoctorRepositoryAsync(appDbContext, appIdentityDb);
            PatientRepositoryAsync = new PatientRepositoryAsync(appDbContext);
            AppointmentRepositoryAsync = new AppointmentRepositoryAsync(appDbContext);
        }


        public async Task<int> CommitAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _appDbContext.DisposeAsync();
        }

    }
}
