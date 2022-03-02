using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Checked.Models.Models
{
    public class ApplicationUser : IdentityUser
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
        public virtual City City { get; set; }
        public int CityId { get; set; }


        [PersonalData]
        public virtual State State { get; set; }
        public int StateId { get; set; }


        [PersonalData]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        [PersonalData]
        public virtual Country Country { get; set; }
        public int CountryId { get; set; }


        [PersonalData]
        public string? OrganizationId { get; set; }

        public virtual Organization? Organization { get; set; }


        public virtual ICollection<Occurrence> Occurrences { get; set; }

    }
}
