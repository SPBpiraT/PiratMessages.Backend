using System.ComponentModel.DataAnnotations;

namespace PiratMessages.WebApi.Models.Users
{
    public class RegisterRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
