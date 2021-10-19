using System.Threading.Tasks;
using Application.Photos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IPhotoAccessor
    {
        Task<PhotoUploadResults> AddPhoto([FromForm(Name = "File")] IFormFile File);

        Task<string> DeletePhoto(string publicId);

    }
}