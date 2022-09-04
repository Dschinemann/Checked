using System.ComponentModel.DataAnnotations.Schema;

namespace Checked.Models.Models.Complement
{
    public class OcurrenceComplement
    {
        public OccurrenceColumnComplement OccurrenceColumnComplement { get; set; }
        public int OccurrenceColumnComplementId { get; set; }
        /**/

        public Occurrence Occurrence { get; set; }
        public string OccurrenceId { get; set; }
        /***/

        public int Id { get; set; }
        public string Value { get; set; }
    }
}
