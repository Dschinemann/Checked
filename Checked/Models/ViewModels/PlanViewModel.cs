using Checked.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class PlanViewModel
    {
        public string PlanId { get; set; }
        public string OccurrenceId { get; set; }
        public string Subject { get; set; }
        public string DeadLine { get; set; }
        public string AccountableId { get; set; }
        public ApplicationUser Accountable { get;set; }
        public string Objective { get; set; }

        public DateTime Goal { get; set; }

        public List<Models.Action> Actions = new List<Models.Action>();
        public IEnumerable<StatusQuantidade>? QuantStatus;

    }

    public class StatusQuantidade
    {
        public string Status { get;set; }
        public int Quantidade { get; set; }
    }
}
