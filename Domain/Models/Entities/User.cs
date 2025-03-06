using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace appointmentApp.Models.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(60)]
        public string Name { get; protected set; }

        protected User() { }
        
        public User(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public void SetName(string name)
        {
            Name = name;
        }

    }
}
