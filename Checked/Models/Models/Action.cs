using Checked.Models.Enums;

namespace Checked.Models.Models
{
    public class Action
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Accountable { get; set; }
        public DateTime Init { get; set; }
        public DateTime Finish { get; set; }
        public DateTime? NewFinish { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set;} = DateTime.Now;
        public string?  What { get; set; }
        public string? Why { get; set; }
        public string? Where { get; set; }
        public string? Who { get; set; }
        public string? When { get; set; }
        public string? How { get; set; }
        public double? HowMuch { get; set; }
        public TP_Status TP_Status { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Organization Organization { get; set; }
        public Occurrence Occurrence { get; set; }
        public virtual int? OccurrenceId { get; set; }

        public Action()
        {

        }       

    }
}
