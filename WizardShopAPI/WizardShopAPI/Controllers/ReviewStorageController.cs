using Microsoft.AspNetCore.Mvc;
using WizardShopAPI.DTOs;
using WizardShopAPI.ResponseDto;
using WizardShopAPI.Services;
using WizardShopAPI.Storage;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewStorageController:ControllerBase
    {
        private readonly IAzureReviewStorage _storage;

        public ReviewStorageController(IAzureReviewStorage storage)
        {
            _storage = storage;
        }

        [HttpPost("{reviewId}")] //if its a review image id: R+ _ + review id
        public async Task<IActionResult> Upload(IFormFile file, int reviewId)
        {
            ImageResponseDto? response = await _storage.UploadAsync(file, reviewId);

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

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetAllImagesForReview(int reviewId)
        {
            List<ImageDto>? files = await _storage.ListAllImagesForReviewAsync(reviewId);

            return StatusCode(StatusCodes.Status200OK, files);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviewImages()
        {
            List<ImageDto>? files = await _storage.ListAsync();

            return StatusCode(StatusCodes.Status200OK, files);
        }
    }


}
