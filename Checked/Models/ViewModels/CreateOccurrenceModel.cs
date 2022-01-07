namespace Checked.Models.ViewModels
{
    public class CreateOccurrenceModel
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public string Harmed { get; set; }
        public string Document { get; set; }
        public string Origin { get; set; }
        public double Cost { get; set; }
        public string Appraiser { get; set; }
    }
}
