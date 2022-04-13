using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class Register
    {
        public string Name { get; set; }
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [MinLength(1)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
