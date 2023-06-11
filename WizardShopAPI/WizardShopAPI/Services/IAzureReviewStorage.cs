using WizardShopAPI.DTOs;

namespace WizardShopAPI.Services
{
    public interface IAzureReviewStorage:IAzureStorage
    {
       // Task<int> GetReviewImageId(int reviewId);

        Task<List<ImageDto>> ListAllImagesForReviewAsync(int reviewId);
        Task<List<string>> ListAllUrisForReviewAsync(int reviewId);
    }
}
