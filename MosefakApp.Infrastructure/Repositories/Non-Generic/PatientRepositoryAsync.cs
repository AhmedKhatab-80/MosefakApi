namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class PatientRepositoryAsync : GenericRepositoryAsync<AppUser>, IPatientRepositoryAsync
    {
        public PatientRepositoryAsync(AppDbContext context) : base(context)
        {
        }
    }
}
