using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Models
{
    public class Organization
    {
        public Organization()
        {
            Users = new HashSet<ApplicationUser>();
            Occurrences = new HashSet<Occurrence>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Occurrence> Occurrences { get; set; }
    }
}
