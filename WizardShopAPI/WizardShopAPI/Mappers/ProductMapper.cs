using WizardShopAPI.DTOs;
using WizardShopAPI.Models;

namespace WizardShopAPI.Mappers
{
    public class ProductMapper
    {
        public static Product ProductDtoToProduct(ref ProductDto dto,ref int productId)
        {
            Product mapped = new Product()
            {
                Id=productId,
                Name=dto.Name,
                Description=dto.Description,
                Price=dto.Price,
                Rating=dto.Rating,
                PhotoId=dto.PhotoId,
                CategoryId=dto.CategoryId,
                Popularity=dto.Popularity,
                Quantity=dto.Quantity
            };
            return mapped;
        }
    }
}
