namespace Checked.Models.ViewModels
{
    public class CreateActionModel
    {
        public string Subject { get; set; }
        public string Accountable { get; set; }
        public DateTime Init { get; set; } = DateTime.Now;
        public DateTime Finish { get; set; } = DateTime.Now;
        public string What { get; set; }
        public string Why { get; set; }
        public string Where { get; set; }
        public string Who { get; set; }
        public string When { get; set; }
        public string How { get; set; }
        public double HowMuch { get; set; }
        public int OccurrenceId { get; set; }
    }
}
