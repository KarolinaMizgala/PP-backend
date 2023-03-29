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
            String status = "Active";
            return new User()
            {
                UserId = id,
                Username = dto.Username,
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Password = dto.Password,   
                Status = status,
                Role = role
            };
        }
    }
}
