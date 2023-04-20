using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WizardShopAPI.DTOs;
using WizardShopAPI.ResponseDto;
using WizardShopAPI.Services;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStorageController : ControllerBase
    {
        private readonly IAzureStorage _storage;
        public ProductStorageController(IAzureStorage storage)
        {
            _storage = storage;
        }

        [HttpGet(nameof(Get))]
        public async Task<IActionResult> Get()
        {
            // Get all files at the Azure Storage Location and return them
            List<ImageDto>? files = await _storage.ListAsync();

            // Returns an empty array if no files are present at the storage container
            return StatusCode(StatusCodes.Status200OK, files);
        }

        [HttpPost("{productId}")] //if its a review image id: R+ _ + review id
        public async Task<IActionResult> Upload(IFormFile file, int productId)
        {
            ImageResponseDto? response = await _storage.UploadAsync(file, productId);

            // Check if we got an error
            if (response.Error == true)
            {
                // We got an error during upload, return an error with details to the client
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                // Return a success message to the client about successfull upload
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> Download(int imageId)
        {
            ImageDto? file = await _storage.DownloadAsync(imageId);

            // Check if file was found
            if (file == null)
            {
                // Was not, return error message to client
                return StatusCode(StatusCodes.Status404NotFound, $"File {imageId} doesn't exist.");
            }
            else
            {
                // File was found, return it to client
                // return File(file.Content, file.ContentType, file.Name,file.Uri);
                return StatusCode(StatusCodes.Status200OK, file);
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            ImageResponseDto response = await _storage.DeleteAsync(productId);

            // Check if we got an error
            if (response.Error == true)
            {
                // Return an error message to the client
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                // File has been successfully deleted
                return StatusCode(StatusCodes.Status200OK, response.Status);
            }
        }

    }
}
