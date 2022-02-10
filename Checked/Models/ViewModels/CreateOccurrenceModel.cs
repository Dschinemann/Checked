using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class CreateOccurrenceModel
    {
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        [Display(Name = "Prejudicado")]
        public string Harmed { get; set; }
        [Display(Name = "Documento/NF")]
        public string Document { get; set; }
        [Display(Name = "Origem")]
        public string Origin { get; set; }
        [Display(Name = "Custo da Ocorrência")]
        public double Cost { get; set; }
        [Display(Name = "Avaliado")]
        public string Appraiser { get; set; }
    }
}
