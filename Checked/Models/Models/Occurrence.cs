using Checked.Models.Enums;
using Checked.Models.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Checked.Models.Models
{
    public class Occurrence
    {
        public string Id { get; set; }

        [Display(Name ="Nome")]
        public string Name { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [Display(Name = "Prejudicado")]
        public string Harmed { get; set; }

        [Display(Name = "Documento/NF")]
        public string? Document { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Custo")]
        public double Cost { get; set; }

        [Display(Name = "Criado em:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Atualizado em:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Avaliador")]
        public string? AppraiserId { get; set; }

        [Display(Name = "Avaliador")]
        public virtual ApplicationUser? Appraiser { get; set; }

        [Display(Name = "Origem")]
        public string Origin { get; set; }
        public string ApplicationUserId { get; set; }
        [Display(Name = "Usuário Cadastro")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string OrganizationId { get; set; }
        [Display(Name = "Empresa")]
        public virtual Organization Organization { get; set; }

        //[ForeignKey("OccurrenceId")]
        [ForeignKey("PlanId")]
        public Plan? Plan { get; set; }
        public virtual string? PlanId { get; set; }

        public TP_StatusOccurence Status { get; set; }
        public virtual int StatusId { get; set; }

        [Display(Name = "Ação corretiva")]
        public string? CorrectiveAction { get; set; }
        [Display(Name = "Status das ações")]
        public string? StatusActions { get; set; }

        public List<Action> Actions { get; set; }

        public Occurrence()
        {
        }

        public Occurrence(string name, string description, string harmed, string? document, double cost, DateTime createdAt, DateTime updatedAt, string? appraiserId, string origin, string applicationUserId,string organizationId, int statusId)
        {
            Name = name;
            Description = description;
            Harmed = harmed;
            Document = document;
            Cost = cost;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            AppraiserId = appraiserId;
            Origin = origin;
            ApplicationUserId = applicationUserId;
            OrganizationId = organizationId;
            StatusId = statusId;
        }
    }
       
}
