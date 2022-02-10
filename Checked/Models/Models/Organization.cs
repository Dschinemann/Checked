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
        [Display(Name ="Nome da Empresa")]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        [Display(Name = "Criado em:")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        [Display(Name = "Atualizado")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Occurrence> Occurrences { get; set; }
    }
}
