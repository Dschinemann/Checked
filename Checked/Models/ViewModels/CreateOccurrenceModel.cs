using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class CreateOccurrenceModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Prejudicado")]
        public string Harmed { get; set; }
        [Required]
        [Display(Name = "Documento/NF")]
        public string Document { get; set; }
        [Required]
        [Display(Name = "Origem")]
        public string Origin { get; set; }
        [Required]
        [Display(Name = "Custo da Ocorrência")]
        public double Cost { get; set; }
        [Required]
        [Display(Name = "Avaliador")]
        public string Appraiser { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public int TypeOccurrence { get; set; }

        [Display(Name = "Informação Adicional 01")]
        public string? Additional1 { get; set; }

        [Display(Name = "Informação Adicional 02")]
        public string? Additional2 { get; set; }

        public string CreatedById { get; set; }

        [Display(Name = "Data da ocorrência")]
        public DateTime DataOccurrence { get; set; }
    }
}
