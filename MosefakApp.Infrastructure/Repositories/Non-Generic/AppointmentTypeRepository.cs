namespace MosefakApp.Infrastructure.Repositories.Non_Generic
{
    public class AppointmentTypeRepository : GenericRepositoryAsync<AppointmentType>, IAppointmentTypeRepository
    {
        private readonly AppDbContext _context;
        public AppointmentTypeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AppointmentType>> GetAppointmentTypes(int doctorId)
        {
            return await _context.AppointmentTypes.ToListAsync();
        }
    }
}
