using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class ForgotModel
    {
        [Required(ErrorMessage ="Field mandatory")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
