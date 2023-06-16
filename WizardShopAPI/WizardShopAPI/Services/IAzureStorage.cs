using WizardShopAPI.DTOs;
using WizardShopAPI.ResponseDto;

namespace WizardShopAPI.Services
{
    public interface IAzureStorage
    {
        /// <summary>
        /// This method uploads a file submitted with the request
        /// </summary>
        /// <param name="file">File for upload</param>
        /// /// <param name="id">Id</param>
        /// <returns>Blob with status</returns>
        Task<ImageResponseDto> UploadAsync(IFormFile file, int id);

        /// <summary>
        /// This method returns a list of all files located in the container
        /// </summary>
        /// <returns>Blobs in a list</returns>
        Task<List<ImageDto>> ListAsync();

        /// <summary>
        /// This method deletes a file with the specified filename
        /// </summary>
        /// <param name="imageId">Filename</param>
        /// <returns>Blob with status</returns>
        Task<bool> DeleteAllImagesFromEntityAsync(int entityId);

        Task<List<string>> GetListOfAllUrisForEntityAsync(int entityId);
    }
}
