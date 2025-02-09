using MosefakApp.Core.Dtos.Resolvers;

namespace MosefakApp.Core.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            // User

            CreateMap<AppUser, UserProfileResponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<UserProfileImageResolver>())
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<Address, AddressUserResponse>().ReverseMap();

            // Doctor

            CreateMap<DoctorRequest, Doctor>()
                .ForMember(dest => dest.NumberOfReviews, opt => opt.MapFrom(src => 0)); // Default value for NumberOfReviews

            CreateMap<ClinicAddressRequest, ClinicAddress>().ReverseMap();
            CreateMap<AppointmentTypeRequest, AppointmentType>().ReverseMap();
            CreateMap<SpecializationRequest, Specialization>().ReverseMap();
            CreateMap<WorkingTimeRequest, WorkingTime>().ReverseMap();


            CreateMap<CompleteDoctorProfileRequest, Doctor>()
                .ForMember(dest => dest.NumberOfReviews, opt => opt.MapFrom(src => 0));
        }
    }
}
