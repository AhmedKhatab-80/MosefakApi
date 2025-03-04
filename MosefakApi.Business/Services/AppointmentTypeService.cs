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

        public async Task<(List<AppointmentTypeResponse> responses, int totalPages)> GetAppointmentTypes(
        int doctorId, int pageNumber = 1, int pageSize = 10)
        {
            (var query, int totalCount) = await _unitOfWork.Repository<AppointmentType>().GetAllAsync(
                x => x.Doctor.AppUserId == doctorId,
                query => query.Include(x => x.Doctor),
                pageNumber,
                pageSize);

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Ensure query is never null (return an empty list instead)
            if (query == null || !query.Any())
            {
                return (new List<AppointmentTypeResponse>(), totalPages);
            }

            var response = _mapper.Map<List<AppointmentTypeResponse>>(query);

            return (response, totalPages);
        }



        public async Task<bool> AddAppointmentType(int doctorId, AppointmentTypeRequest request)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().FirstOrDefaultAsync(x => x.AppUserId == doctorId);
            
            if (doctor is null)
                throw new ItemNotFound("Doctor does not exist");

            var appointmentType = _mapper.Map<AppointmentType>(request);
            appointmentType.DoctorId = doctor.Id;

            await _unitOfWork.Repository<AppointmentType>().AddEntityAsync(appointmentType);
            var rowsAffected = await _unitOfWork.CommitAsync();

            if (rowsAffected <= 0)
                throw new BadRequest($"can't add new appointment type");

            return true;
        }

        public async Task<bool> EditAppointmentType(int appointmentTypeId, AppointmentTypeRequest request)
        {
            var appointmentType = await _unitOfWork.Repository<AppointmentType>().FirstOrDefaultAsync(x => x.Id == appointmentTypeId);

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
            var appointmentType = await _unitOfWork.Repository<AppointmentType>().FirstOrDefaultAsync(x => x.Id == appointmentTypeId);

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
