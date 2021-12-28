using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Mandatory Field")]
        [EmailAddress(ErrorMessage ="Invalid Format")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password Mandatory")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Remember-me")]
        public bool RememberMe { get; set; }
    }
}
