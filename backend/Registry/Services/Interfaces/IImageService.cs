namespace Registry.Services.Interfaces;

public interface IImageService
{
    public Task<string> UploadImage(Stream destinationImage, CancellationToken token = default);
}