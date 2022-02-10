using Checked.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class CreateActionViewModel
    {
        public int? Id { get; set; }
        public int PlanId { get; set; }
        [Display(Name = "What? (O que?)")]
        public string? What { get; set; }
        [Display(Name = "Why? (Porquê?)")]
        public string? Why { get; set; }
        [Display(Name = "Where? (Onde?)")]
        public string? Where { get; set; }
        [Display(Name = "Who? (Quem?)")]
        public string? Who { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Início")]
        public DateTime Init { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        [Display(Name = "Previsão para término")]
        public DateTime Finish { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Nova previsão")]
        public DateTime NewFinish { get; set; } = DateTime.Now;
        [Display(Name = "How? (Como?)")]
        public string? How { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "How Much? (Quanto Custa?)")]
        public double? HowMuch { get; set; }
        public TP_Status status { get; set; }
    }
}
