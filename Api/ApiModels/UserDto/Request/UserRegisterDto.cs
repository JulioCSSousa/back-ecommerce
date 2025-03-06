using appointmentApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dto.UserDto.Request
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password do not match")]
        public string ConfirmPassword { get; set; }

    }
}
