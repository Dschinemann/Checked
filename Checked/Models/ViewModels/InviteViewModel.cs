using Checked.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class InviteViewModel
    {
        [Required]
        public virtual string OrganizationId { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        public List<UsersInRoleViewModel>? users { get; set; }
    }
}
