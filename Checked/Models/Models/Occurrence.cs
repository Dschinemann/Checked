using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Models
{
    public class Occurrence
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Harmed { get; set; }
        public string? Document { get; set; }

        [DataType(DataType.Currency)]
        public double Cost { get; set; }
        [Display(Name = "Created At")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "Updated At")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Appraiser")]
        public string? AppraiserId { get; set; }
        public virtual ApplicationUser? Appraiser { get; set; }

        public string Origin { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

    }
}
