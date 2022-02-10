using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class CreatePlanViewModel
    {
        [Required]
        [Display(Name ="Motivo")]
        public string Subject { get; set; }
        [Required]
        [Display(Name = "Responsável")]
        public string Accountable { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Goal { get; set; }
        [Required]
        [Display(Name = "Objetivo")]
        public string Objective { get; set; }
        [Required]
        public int OccurrenceId { get; set; }

    }
}
