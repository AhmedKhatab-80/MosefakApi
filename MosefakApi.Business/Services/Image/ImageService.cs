namespace MosefakApi.Business.Services.Image
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _basePath;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _basePath = Path.Combine(_environment.WebRootPath, "images");
        }

        public async Task<string> UploadImageOnServer(string folderName, IFormFile image, bool deleteIfExist = false, string oldPath = null, CancellationToken cancellationToken = default)
        {

            var folderPath = Path.Combine(_basePath, folderName);
            Directory.CreateDirectory(folderPath); // Ensure the folder exists

            if (deleteIfExist && oldPath is not null)
            {
                await RemoveImage($"{folderName}/{oldPath}");
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

            var fullPath = Path.Combine(folderPath, uniqueFileName);

            using var stream = new FileStream(fullPath, FileMode.OpenOrCreate);
            await image.CopyToAsync(stream, cancellationToken); // will put uploaded file in this path in wwwroot
            stream.Close();

            return uniqueFileName;
        }

        public Task RemoveImage(string oldPath)
        {
            if (string.IsNullOrWhiteSpace(oldPath))
            {
                return Task.CompletedTask;
            }

            string imagePath = Path.Combine(_basePath, oldPath);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            return Task.CompletedTask;
        }
    }

}
