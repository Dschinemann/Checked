using Checked.Models.Enums;

namespace Checked.Models.ViewModels
{
    public class DashViewModel
    {
        public string OrganizationName { get; set; }
        public List<ActionsResume> resume { get; set; }
        public PlansResume PlanResume { get; set; }
        public OccurrenceResume OccurrenceResume { get; set; }
        public DeadLineActions DeadLineActions { get; set; }
    }
    public class ActionsResume
    {
        public TP_Status Tipo { get; set; }
        public int Quantidade { get; set; }
    }
    public class PlansResume
    {
        public int PlanCriados { get; set; }
        public int QuantEncerrados { get; set; }
        public double ? CustoTotal { get; set; }
    }
    public class OccurrenceResume
    {
        public int Count { get; set; }
        public double TotalCost { get; set; }
    }

    public class DeadLineActions
    {
        public string Name { get; set; }
        public DateTime DeadLine { get; set; }
    }
}
