using appointmentApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dto.UserDto.Request
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
