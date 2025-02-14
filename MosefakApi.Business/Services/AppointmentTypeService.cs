namespace MosefakApi.Business.Services
{
    public class AppointmentTypeService : IAppointmentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AppointmentTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<AppointmentTypeResponse>> GetAppointmentTypes(int doctorId)
        {
            var query = await _unitOfWork.GetCustomRepository<AppointmentTypeRepository>().GetAppointmentTypes(doctorId);

            if (query is null)
                return new List<AppointmentTypeResponse>();

            var response = _mapper.Map<List<AppointmentTypeResponse>>(query);

            return response;
        }
    }
}
