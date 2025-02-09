namespace MosefakApp.Core.IUnit
{
    public interface IUnitOfWork<T> : IAsyncDisposable where T : class
    {
        IGenericRepositoryAsync<T> RepositoryAsync { get; }
        IDoctorRepositoryAsync DoctorRepositoryAsync { get; }
        IPatientRepositoryAsync PatientRepositoryAsync { get; }
        IAppointmentRepositoryAsync AppointmentRepositoryAsync { get; }
        Task<int> CommitAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
