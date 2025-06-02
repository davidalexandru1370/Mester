using Microsoft.AspNetCore.Mvc;
using Registry.Services.Interfaces;

namespace Registry.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private IImageService ImageService;
        public ImageController(IImageService imageService)
        {
            ImageService = imageService;
        }

        [HttpPost]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> UploadImage(IFormFile image, CancellationToken token)
        {
            string result;
            try
            {
                result = await ImageService.UploadImage(image.OpenReadStream(), token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(result);
        }
    }
}
