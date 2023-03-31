using WizardShopAPI.DTOs;
using WizardShopAPI.Models;

namespace WizardShopAPI.Mappers
{
    public class AddressMapper
    {
        public static Address AddressDtoToAddress(AddressDto addressDto, int userId,int addressId)
        {
            Address address = new Address() 
            {
                AdressId=addressId,
                UserId=userId,
                ZipCode=addressDto.ZipCode,
                City=addressDto.City,
                Street=addressDto.Street,
                HouseNumber=addressDto.HouseNumber,
                ApartmentNumber=addressDto.ApartmentNumber
            };
            return address;
        }
    }
}
