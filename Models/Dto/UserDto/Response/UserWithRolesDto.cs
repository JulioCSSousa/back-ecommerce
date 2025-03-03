using appointmentApp.Models.Entities;

namespace EcommerceApi.Models.Dto.UserDto.Response
{
    public class UserWithRolesDto : User
    {
        public string Name { get; set; }
        public string? Role { get; set; }

    }
}
