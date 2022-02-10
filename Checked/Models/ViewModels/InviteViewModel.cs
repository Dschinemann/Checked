using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class InviteViewModel
    {
        [Required]
        public virtual int OrganizationId { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
