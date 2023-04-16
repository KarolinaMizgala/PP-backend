using System.ComponentModel.DataAnnotations.Schema;

namespace WizardShopAPI.DTOs
{
    public class ImageDto
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
    }
}
