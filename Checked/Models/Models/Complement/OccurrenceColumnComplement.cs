namespace Checked.Models.Models.Complement
{
    public class OccurrenceColumnComplement
    {
        public Organization Organization { get; set; }
        public string OrganizationId { get; set; }

        public int Id { get; set; }
        public string ColumnTitle { get; set; } = "Nome da Coluna";
    }
}
