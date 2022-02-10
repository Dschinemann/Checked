using System.ComponentModel.DataAnnotations.Schema;

namespace Checked.Models.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Accountable { get; set; }

        public DateTime Forecast { get; set; }
        public DateTime Goal { get; set; }
        public double CostTotal { get; set; }

        public string Objective { get; set; }

        public int OccurrenceId { get; set; }
        public virtual Occurrence Occurrence { get; set; }

        public int organizationId {get; set;}
        public virtual Organization organization { get; set; }

        public List<Action> Actions { get; set; } = new List<Action>();

    }
}
