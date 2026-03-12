using Microsoft.AspNetCore.Http;

namespace VoteMe.Application.Interface.IServices
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder, Guid EntityId);
    }
}
