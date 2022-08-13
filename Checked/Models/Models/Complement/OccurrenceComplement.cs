namespace Checked.Models.Models.Complement
{
    public class OccurrenceComplement
    {
        public Occurrence Occurrence { get; set; }
        public string OccurrenceId { get; set; }
        public Organization Organization { get; set; }
        public string OrganizationId { get; set; }

        /***/
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ComplementTitle { get; set; }
        public string ComplementDescription { get; set; }
    }
}
