using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dto.UserDto.Request
{
    public class PasswordRecoveryDto
    {
        public class PasswordRecoverViewModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
