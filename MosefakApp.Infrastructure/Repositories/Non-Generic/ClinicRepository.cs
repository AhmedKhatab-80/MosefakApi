namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class ClinicRepository : GenericRepositoryAsync<Clinic>, IClinicRepository
    {
        public ClinicRepository(AppDbContext context) : base(context)
        {
        }
    }
}
