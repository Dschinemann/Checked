using System.ComponentModel.DataAnnotations;

namespace Checked.Models.FilterModels
{
    public class OccurrencesFilter
    {
        public int? TP_OcorrenciaId { get; set; }
        public string? Description { get; set; }
        public string? Harmed { get; set; }
        public string? Document { get; set; }
        public string? Additional1 { get; set; }
        public string? Additional2 { get; set; }
        public double? Cost { get; set; }
        public string? AppraiserId { get; set; }
        public string? Origin { get; set; }
        public int? StatusId { get; set; }
        public string? StatusActions { get; set; }
        public string? CorrectiveAction { get; set; }

        public int? TipoFiltroData { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? EndDate { get; set; }

    }
}
