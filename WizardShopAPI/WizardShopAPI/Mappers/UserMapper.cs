using WizardShopAPI.DTOs;
using WizardShopAPI.Models;

namespace WizardShopAPI.Mappers
{
    public class UserMapper
    {
        public static User RegisterDtoToUser(ref RegisterDto dto, ref int id)
        {
            if (dto == null)
            {
                return null;
            }
            String role = "User";
            String status = "Unactivated";
            return new User()
            {
                UserId = id,
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password,   
                Status = status,
                Role = role
            };
        }
    }
}
