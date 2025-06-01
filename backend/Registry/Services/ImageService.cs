using Firebase.Auth;
using Firebase.Storage;
using Registry.Errors.Services;
using Registry.Services.Interfaces;

namespace Registry.Services;

public class ImageService : IImageService
{
    private static readonly string ApiKey = "AIzaSyBG_DScFgkNUOooWMoGvv2D1C7V85gKg5A";
    private static readonly string Bucket = "destinationbucketimages.appspot.com";
    private static readonly string AuthEmail = "admin@gmail.com";
    private static readonly string AuthPassword = "123456789";

    public ImageService()
    {

    }

    public async Task<string> UploadImage(IFormFile destinationImage)
    {
        var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
        var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

        var cancellation = new CancellationTokenSource();
        await using var stream = destinationImage.OpenReadStream();

        var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel =
                        true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
            .Child(Guid.NewGuid().ToString())
            .PutAsync(stream, cancellation.Token);

        task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

        // cancel the upload
        // cancellation.Cancel();

        try
        {
            // error during upload will be thrown when you await the task
            var link = await task;
            return link;
        }
        catch (Exception ex)
        {
            throw new ServiceException("Error while uploading the image", ex);
        }
    }
}