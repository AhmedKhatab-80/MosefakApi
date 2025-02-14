namespace MosefakApp.Core.IRepositories.Non_Generic
{
    public interface IAppointmentTypeRepository : IGenericRepositoryAsync<AppointmentType>
    {
        Task<List<AppointmentType>> GetAppointmentTypes(int doctorId);
    }
}
