using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace caobaModeloFabricacion.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadAsync(ImageUploadParams uploadParams);
        string GetThumbnailUrl(string publicId, Transformation transformation);
    }
}
