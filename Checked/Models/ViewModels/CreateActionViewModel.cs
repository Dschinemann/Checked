using Checked.Models.Enums;
using Checked.Models.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class CreateActionViewModel
    {
        public string? Id { get; set; }
        public string PlanId { get; set; }

        public string OccurrenceId { get; set; }


        [Display(Name = "What? (O que?)")]
        public string What { get; set; }
        [Display(Name = "Why? (Porquê?)")]
        public string? Why { get; set; }
        [Display(Name = "Where? (Onde?)")]
        public string? Where { get; set; }
        [Display(Name = "Who? (Quem?)")]
        public string? WhoId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Início")]
        public DateTime Init { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Previsão para término")]
        [Remote("isValideFinishDate", "Actions", AdditionalFields = nameof(Goal))]
        public DateTime Finish { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Nova previsão")]
        [Remote("isValideFinishDate", "Actions", AdditionalFields = nameof(Goal))]
        public DateTime NewFinish { get; set; } = DateTime.Now;

        [Remote("isValideFinishDate", "Actions", AdditionalFields = nameof(NewFinish))]
        public DateTime Goal { get; set; }


        [Display(Name = "How? (Como?)")]
        public string? How { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "How Much? (Quanto Custa?)")]
        public double? HowMuch { get; set; }
        public TP_Status? Status { get; set; }
        public virtual int StatusId { get; set; }

        public SelectList? Users {get;set;}
    }
}
