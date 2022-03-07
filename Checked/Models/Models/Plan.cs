using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Checked.Models.Models
{
    public class Plan
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Display(Name ="Assunto")]
        public string Subject { get; set; }

        [Display(Name = "Responsável")]
        public string AccountableId { get; set; }   
        public virtual ApplicationUser? Accountable { get; set; }        
        public DateTime Forecast { get; set; }
        [Display(Name = "Previsão para finalizar todas as ações")]
        public DateTime Goal { get; set; }
        [Display(Name = "Custo Total das ações")]
        public double CostTotal { get; set; }
        [Display(Name = "Objetivo")]
        public string Objective { get; set; }

        public string OccurrenceId { get; set; }

        [ForeignKey("OccurrenceId ")]
        public virtual Occurrence Occurrence { get; set; }

        public string organizationId {get; set;}
        public virtual Organization Organization { get; set; }

        public ApplicationUser CreatedBy { get; set; }
        public virtual string CreatedById { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public List<Action> Actions { get; set; } = new List<Action>();

    }
}
