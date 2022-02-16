using Checked.Models.Models;
namespace Checked.Models.ViewModels
{
    public class PlanViewModel
    {
        public string PlanId { get; set; }
        public string OccurrenceId { get; set; }
        public string Subject { get; set; }
        public string DeadLine { get; set; }
        public string Accountable { get; set; }
        public string Objective { get; set; }

        public List<Models.Action> Actions = new List<Models.Action>();

    }
}
