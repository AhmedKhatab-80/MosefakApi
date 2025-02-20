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

        public async Task<List<AppointmentTypeResponse>?> GetAppointmentTypes(int doctorId)
        {
            var query = await _unitOfWork.Repository<AppointmentType>().GetAllAsync(x=> x.Doctor.Id == doctorId, ["Doctor"]);

            if (query is null)
                return null;

            var response = _mapper.Map<List<AppointmentTypeResponse>>(query);

            return response;
        }


        public async Task<bool> AddAppointmentType(int doctorId, AppointmentTypeRequest request)
        {
            var isExist = await _unitOfWork.Repository<Doctor>().AnyAsync(x => x.AppUserId == doctorId);
            
            if (!isExist)
                throw new ItemNotFound("Doctor does not exist");

            var appointmentType = _mapper.Map<AppointmentType>(request);

            await _unitOfWork.Repository<AppointmentType>().AddEntityAsync(appointmentType);
            var rowsAffected = await _unitOfWork.CommitAsync();

            if (rowsAffected <= 0)
                throw new BadRequest($"can't add new appointment type");

            return true;
        }

        public async Task<bool> EditAppointmentType(int appointmentTypeId, AppointmentTypeRequest request)
        {
            var appointmentType = await _unitOfWork.Repository<AppointmentType>().FirstOrDefaultASync(x => x.Id == appointmentTypeId);

            if (appointmentType is null)
                throw new ItemNotFound("Appointment Type does not exist");

            appointmentType.VisitType = request.VisitType;
            appointmentType.Duration = request.Duration;
            appointmentType.ConsultationFee = request.ConsultationFee;

            await _unitOfWork.Repository<AppointmentType>().UpdateEntityAsync(appointmentType);
            var rowsAffected = await _unitOfWork.CommitAsync();

            if (rowsAffected <= 0)
                throw new BadRequest($"can't edit new appointment type");

            return true;
        }


        public async Task<bool> DeleteAppointmentType(int appointmentTypeId)
        {
            var appointmentType = await _unitOfWork.Repository<AppointmentType>().FirstOrDefaultASync(x => x.Id == appointmentTypeId);

            if (appointmentType is null)
                throw new ItemNotFound("Appointment Type does not exist");

            await _unitOfWork.Repository<AppointmentType>().DeleteEntityAsync(appointmentType);
            var rowsAffected = await _unitOfWork.CommitAsync();

            if (rowsAffected <= 0)
                throw new BadRequest($"can't delete new appointment type");

            return true;
        }

        

    
    }
}
