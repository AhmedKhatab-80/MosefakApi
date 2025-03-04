namespace MosefakApp.Core.IServices
{
    public interface IContactUsService
    {
        Task<PaginatedResponse<ContactUsResponse>> GetMessages(int pageNumber = 1, int pageSize = 10);
        Task<bool> CreateContactMessage(int userId, ContactUsRequest request);
    }
}
