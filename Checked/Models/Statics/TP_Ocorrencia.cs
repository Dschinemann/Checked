using Checked.Models.Models;

namespace Checked.Models.Types
{
    public class TP_Ocorrencia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual Organization? Organization { get; set; }
        public string OrganizationId { get; set; }

        public ApplicationUser? CreatedBy { get; set; }
        public virtual string CreatedById { get; set; }
    }
}