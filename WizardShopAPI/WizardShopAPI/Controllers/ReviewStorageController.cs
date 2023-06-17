using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WizardShopAPI.DTOs;
using WizardShopAPI.Models;
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
        private readonly WizardShopDbContext _context;
        public ReviewStorageController(IAzureReviewStorage storage, WizardShopDbContext context)
        {
            _storage = storage;
            _context = context;
        }

        [Authorize]
        [HttpPost("{reviewId}")] 
        public async Task<IActionResult> Upload(IFormFile file, int reviewId)
        {
            if (_context.Reviews == null)
            {
                return BadRequest();

            }

            var review = _context.Reviews.Where(r => r.ReviewId == reviewId).SingleOrDefault();
            if (review == null)
            {
                return BadRequest();
            }

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
            List<string>? files = await _storage.GetListOfAllUrisForEntityAsync(reviewId);

            return StatusCode(StatusCodes.Status200OK, files);
        }

        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteAllImagesForReview(int reviewId)
        {
            if (_context.Reviews == null)
            {
                return BadRequest();

            }

            var review = _context.Reviews.Where(r => r.ReviewId == reviewId).SingleOrDefault();
            if (review == null)
            {
                return BadRequest();
            }

            bool response = await _storage.DeleteAllImagesFromReviewAsync(reviewId);
            if (!response)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviewImages()
        {
            List<ImageDto>? files = await _storage.ListAsync();

            return StatusCode(StatusCodes.Status200OK, files);
        }
    }
}
