using System.ComponentModel.DataAnnotations;

namespace WebApiTest.Infrastructure.Models
{
    public class RegistrationModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
