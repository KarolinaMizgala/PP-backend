namespace WizardShopAPI.Services
{
    public interface IAzureReviewStorage : IAzureStorage
    {
        Task<bool> DeleteAllImagesFromReviewAsync(int reviewId);
    }
}
