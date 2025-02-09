namespace MosefakApp.Core.Dtos.Resolvers
{
    public class UserProfileImageResolver : IValueResolver<AppUser, UserProfileResponse, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProfileImageResolver(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public string Resolve(AppUser source, UserProfileResponse destination, string destMember, ResolutionContext context)
        {
            if(source != null && !string.IsNullOrEmpty(source.ImagePath))
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}/"; // Get Base URL dynamically

                return $"{baseUrl}{source.ImagePath.Replace("\\", "/")}";
            }
            return string.Empty;
        }
    }
}
