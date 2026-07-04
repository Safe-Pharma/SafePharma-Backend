using Microsoft.AspNetCore.Http;

namespace SafePharma.BLL
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
