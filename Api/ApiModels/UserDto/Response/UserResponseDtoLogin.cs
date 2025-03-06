using appointmentApp.Models.Entities;

namespace EcommerceApi.Models.Dto.UserDto.Response
{
    public class UserResponseDtoLogin
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token {get; set; }


    }
}
