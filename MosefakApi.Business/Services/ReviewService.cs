namespace MosefakApi.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly string _basePath;
        public ReviewService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
            _basePath = _configuration["BaseUrl"]!;
        }

        /// <summary>
        /// Retrieves all reviews for a specific doctor.
        /// </summary>
        public async Task<List<ReviewResponse>> GetAllReviews(int doctorId)
        {
            var reviews = await _unitOfWork.Repository<Review>().GetAllAsync(x => x.DoctorId == doctorId);
            if (!reviews.Any()) return new List<ReviewResponse>(); // ✅ Return empty list instead of null

            // 🔹 Retrieve all users in a single query
            var userIds = reviews.Select(x => x.AppUserId).ToHashSet();
            var usersDict = await _userManager.Users
                .Where(x => userIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.ImagePath
                });

            // 🔹 Use LINQ for better performance
            return reviews.Select(review => new ReviewResponse
            {
                Id = review.Id.ToString(),
                Comment = review.Comment,
                Rate = review.Rate,
                CreatedAt = review.CreatedAt,
                FullName = usersDict.TryGetValue(review.AppUserId, out var user)
                    ? $"{user.FirstName} {user.LastName}"
                    : "Unknown",
                ImagePath = usersDict.TryGetValue(review.AppUserId, out var userData)
                    ? $"{_basePath}{userData.ImagePath}"
                    : $"{_basePath}default.jpg"
            }).ToList();
        }

        /// <summary>
        /// Adds a new review for a doctor.
        /// </summary>
        public async Task<bool> AddReview(int patientId, int doctorId, ReviewRequest request, CancellationToken cancellationToken = default)
        {
            if (!await _unitOfWork.GetCustomRepository<DoctorRepositoryAsync>().AnyAsync(x => x.Id == doctorId))
                throw new ItemNotFound("Doctor does not exist.");

            var review = new Review
            {
                DoctorId = doctorId,
                AppUserId = patientId,
                Rate = request.Rate,
                Comment = request.Comment
            };

            await _unitOfWork.Repository<Review>().AddEntityAsync(review);
            return await _unitOfWork.CommitAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        public async Task<bool> EditReview(int reviewId, ReviewRequest request, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Repository<Review>().FirstOrDefaultASync(x => x.Id == reviewId)
                ?? throw new ItemNotFound("Review does not exist.");

            review.Comment = request.Comment;
            review.Rate = request.Rate;

            await _unitOfWork.Repository<Review>().UpdateEntityAsync(review);
            return await _unitOfWork.CommitAsync(cancellationToken) > 0;
        }

        /// <summary>
        /// Deletes a review.
        /// </summary>
        public async Task<bool> RemoveReview(int reviewId, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Repository<Review>().FirstOrDefaultASync(x => x.Id == reviewId)
                ?? throw new ItemNotFound("Review does not exist.");

            await _unitOfWork.Repository<Review>().DeleteEntityAsync(review);
            return await _unitOfWork.CommitAsync(cancellationToken) > 0;
        }
    }
}
