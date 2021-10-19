using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        private readonly Cloudinary _cloudinary;
        private readonly IWebHostEnvironment _env;
        public PhotoAccessor(IOptions<CloudinaryConfig> config, IWebHostEnvironment env)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _env = env;
            _cloudinary = new Cloudinary(account);
        }

        // private bool CheckFileExists(IFormFile file)
        // {
        //     var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
        //     return (extension == ".jpg" || extension == ".png"); // Change the extension based on your need
        // }

        public async Task<PhotoUploadResults> AddPhoto([FromForm(Name = "File")] IFormFile File)
        {
            if (File?.Length > 0)
            {
                await using var stream = File.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(File.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill") // style image 
                };

                // upload cloud service to upload image: Cloudinary 
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return new PhotoUploadResults
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };
            }

            return null;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}