using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using WizardShopAPI.DTOs;
using WizardShopAPI.Models;
using WizardShopAPI.ResponseDto;
using WizardShopAPI.Services;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStorageController : ControllerBase
    {
        private readonly IAzureStorage _storage;
        private readonly WizardShopDbContext _context;
        public ProductStorageController(IAzureStorage storage, WizardShopDbContext context)
        {
            _storage = storage;
            _context = context;
        }

        [HttpGet(nameof(Get))]
        public async Task<IActionResult> Get()
        {
            // Get all files at the Azure Storage Location and return them
            List<ImageDto>? files = await _storage.ListAsync();

            // Returns an empty array if no files are present at the storage container
            return StatusCode(StatusCodes.Status200OK, files);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> Upload(IFormFile file, int productId)
        {
            if (!_context.Products.Any(p => p.Id == productId)) return BadRequest("invalid product id");

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

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAllProductImages(int productId)
        {
            if (!_context.Products.Any(p => p.Id == productId)) return BadRequest("invalid product id");
            List<string>? files = await _storage.GetListOfAllUrisForEntityAsync(productId);

            return StatusCode(StatusCodes.Status200OK, files);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            if (!_context.Products.Any(p => p.Id == productId)) return BadRequest("invalid product id");

            bool response = await _storage.DeleteAllImagesFromEntityAsync(productId);

            // Check if we got an error
            if (!response)
            {
                // Return an error message to the client
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
            {
                // File has been successfully deleted
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }

}
