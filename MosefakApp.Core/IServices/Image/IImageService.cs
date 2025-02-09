namespace MosefakApp.Core.IServices.Image
{
    public interface IImageService
    {
        Task<string> UploadImageOnServer(string folderName, IFormFile image, bool deleteIfExist = false, string oldPath = null!, CancellationToken cancellationToken = default);
        Task RemoveImage(string oldPath);
    }
}
