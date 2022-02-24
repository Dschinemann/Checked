using Checked.Models.Models;

namespace Checked.Models.Types
{
    public class TP_Ocorrencia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual Organization? Organization { get; set; }
        public string OrganizationId { get; set; }
    }
}