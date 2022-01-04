using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Checked.Models.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ApplicationUser()
        {
             this.Occurrences = new HashSet<Occurrence>();
        }

        [Required]
        [PersonalData]
        public string Name { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime CreatedAt { get; set; }
        [PersonalData]
        public string City { get; set; }
        [PersonalData]
        public string Region { get; set; }
        [PersonalData]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        [PersonalData]
        public string Country { get; set; }

        [PersonalData]
        public int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
        [InverseProperty("ApplicationUser")]
        public virtual ICollection<Occurrence> Occurrences { get; set;}
        
    }
}
