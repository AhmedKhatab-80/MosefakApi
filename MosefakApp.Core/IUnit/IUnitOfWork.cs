using MosefakApp.Core.IRepositories.Non_Generic;

namespace MosefakApp.Core.IUnit
{
    public interface IUnitOfWork<T> : IAsyncDisposable where T : class
    {
        IGenericRepositoryAsync<T> RepositoryAsync { get; }
        IDoctorRepositoryAsync DoctorRepositoryAsync { get; }
        IPatientRepositoryAsync PatientRepositoryAsync { get; }
        IAppointmentRepositoryAsync AppointmentRepositoryAsync { get; }
        Task<int> CommitAsync();
    }
}
