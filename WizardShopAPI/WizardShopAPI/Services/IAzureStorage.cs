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
        /// /// <param name="imageId">imageId</param>
        /// <returns>Blob with status</returns>
        Task<ImageResponseDto> UploadAsync(IFormFile file, int imageId);

        /// <summary>
        /// This method downloads a file with the specified filename
        /// </summary>
        /// <param name="imageId">Filename</param>
        /// <returns>Blob</returns>
        Task<ImageDto> DownloadAsync(int imageId);

        /// <summary>
        /// This method deletes a file with the specified filename
        /// </summary>
        /// <param name="imageId">Filename</param>
        /// <returns>Blob with status</returns>
        Task<ImageResponseDto> DeleteAllsFromReviewImageAsync(int imageId);

        /// <summary>
        /// This method returns a list of all files located in the container
        /// </summary>
        /// <returns>Blobs in a list</returns>
        Task<List<ImageDto>> ListAsync();
    }
}
