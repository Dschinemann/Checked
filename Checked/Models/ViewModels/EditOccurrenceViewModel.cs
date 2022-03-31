using Checked.Models.Enums;
using Checked.Models.Models;
using Checked.Models.Types;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class EditOccurrenceViewModel
    {

        public string Id { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public int TypeOccurrence { get; set; }
        public string Description { get; set; }
        [Display(Name = "Prejudicado")]
        public string Harmed { get; set; }
        [Display(Name = "Documento/NF")]
        public string? Document { get; set; }

        [Display(Name = "Custo")]               
        public double Cost { get; set; }

        [Display(Name ="Avaliador")]
        public string? AppraiserId { get; set; }

        public string ApplicationUserId { get; set; }
        [Display(Name = "Origem")]
        public string Origin { get; set; }
        public TP_StatusOccurence  Status { get; set; }
        [Display(Name = "Ação Corretiva")]
        public string? CorretiveActions { get; set; }

        public string OrganizationId { get; set; }

        [Display(Name = "Informação Adicional 01")]
        public string? Additional1 { get; set; }

        [Display(Name = "Informação Adicional 02")]
        public string? Additional2 { get; set; }

        public EditOccurrenceViewModel(
            string id, 
            int type, 
            string description,
            string harmed, 
            string? document, 
            double cost, 
            string? appraiserId, 
            string applicationUserId,
            string origin, 
            string organizationId,
            TP_StatusOccurence  status,
            string? corretiveActions
            )
        {
            Id = id;
            TypeOccurrence = type;
            Description = description;
            Harmed = harmed;
            Document = document;
            Cost = cost;
            AppraiserId = appraiserId;
            ApplicationUserId = applicationUserId;
            Origin = origin;
            OrganizationId = organizationId;
            CorretiveActions = corretiveActions;
            Status = status;
        }

        public EditOccurrenceViewModel() { }

    }
}
