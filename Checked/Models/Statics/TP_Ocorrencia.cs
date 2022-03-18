using Checked.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Types
{
    public class TP_Ocorrencia
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }
        
        public virtual Organization? Organization { get; set; }
        public string OrganizationId { get; set; }

        public ApplicationUser? CreatedBy { get; set; }
        public virtual string CreatedById { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}