namespace Registry.Services.Interfaces;

public interface IImageService
{
    public Task<string> UploadImage(IFormFile destinationImage);
}