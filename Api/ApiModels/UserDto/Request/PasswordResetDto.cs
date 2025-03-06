using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dto.UserDto.Request
{
    public class PasswordResetDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
