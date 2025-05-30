using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace caobaModeloFabricacion.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<ImageUploadResult> UploadAsync(ImageUploadParams uploadParams)
        {
            return await _cloudinary.UploadAsync(uploadParams);
        }

        public string GetThumbnailUrl(string publicId, Transformation transformation)
        {
            return _cloudinary.Api.UrlImgUp
                .Transform(transformation)
                .BuildUrl(publicId);
        }
    }
}
