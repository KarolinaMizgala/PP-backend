using WizardShopAPI.DTOs;

namespace WizardShopAPI.ResponseDto
{
    public class ImageResponseDto
    {
        public string? Status { get; set; }
        public bool Error { get; set; }
        public ImageDto Image { get; set; }

        public ImageResponseDto()
        {
            Image = new ImageDto();
        }
    }
}
