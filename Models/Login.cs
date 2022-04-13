using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models
{
    public class Login
    {
        [Required]
        [MinLength(1)]
        [DataType(DataType.Text)]
        public string UserName { get; set; }
        [Required]
        [MinLength(1)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
