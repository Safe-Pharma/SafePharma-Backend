using Microsoft.AspNetCore.Http;

namespace SafePharma.BLL
{
    public class UploadLogoDto
    {
        public IFormFile Logo { get; set; }
    }
}