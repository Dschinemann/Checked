using Checked.Models.Enums;
using Checked.Models.Models;
using Checked.Models.Types;

namespace Checked.Models.ViewModels
{
    public class DashViewModel
    {
        public string UserName { get; set; }
        public List<ActionsSummary> Summary { get; set; }
        public PlansSummary PlanSummary { get; set; }
        public OccurrenceResume OccurrenceSummary { get; set; }
        public DeadLineActions DeadLineActions { get; set; }
        public List<SummaryPerWeek> WeekSummary { get; set; }
        public List<SummaryOccurrencesPerStatus> SummaryOccurrencesPerStatuses { get; set; }
        public List<SummarryOccurrencePerName> SummarryOccurrencePerNames { get; set; }

        public List<Occurrence> Occurrences { get; set; }
    }
    public class ActionsSummary
    {
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
    }
    public class PlansSummary
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

    public class SummaryPerWeek
    {
        public string Week { get; set; }
        public double Cost { get; set; }
    }

    public class SummaryOccurrencesPerStatus
    {
        public string Status { get; set; }
        public int Quantidade { get; set; }
    }

    public class SummarryOccurrencePerName
    {
        public string Name { get; set; }
        public int Month { get; set; }
        public double Cost { get; set; }
    }
}
