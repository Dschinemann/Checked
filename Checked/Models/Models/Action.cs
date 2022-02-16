using Checked.Models.Enums;
using Checked.Models.Types;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Models
{
    public class Action
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [DataType(DataType.Date)]
        [Display(Name ="Início")]
        public DateTime Init { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Previsão para finalizar")]
        public DateTime Finish { get; set; }

        [Required]
        [Display(Name = "Novo Prazo")]
        [DataType(DataType.Date)]
        public DateTime NewFinish { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Criado em")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Atualizado em")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set;} = DateTime.Now;

        [Display(Name = "What? (O que?)")]
        public string?  What { get; set; }
        [Display(Name = "Why? (Porquê?)")]
        public string? Why { get; set; }
        [Display(Name = "Where? (Onde?)")]
        public string? Where { get; set; }
        [Display(Name = "Who? (Quem?)")]
        public string? Who { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "When? (Quando?)")]
        public DateTime? When { get; set; }
        [Display(Name = "How? (Como?)")]
        public string? How { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "How Much? (Quanto Custa?)")]
        public double? HowMuch { get; set; }
        [Display(Name = "Status")]
        public TP_Status TP_Status { get; set; }
        public virtual int TP_StatusId { get; set; }
        
        public Occurrence Occurrence { get; set; }
        public virtual string OccurrenceId { get; set; }

        public Plan Plan { get; set; }

        public virtual string PlanId { get; set; }

        public Organization Organization { get; set; }
        public virtual string OrganizationId { get; set; }

        public Action()
        {

        }       

    }
}
