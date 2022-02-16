using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Types
{
    public class TP_Status
    {
        public int Id { get; set; }

        [Display(Name ="Status")]
        public string? Name { get; set; }
    }
    public enum TP_StatusEnum : int
    {
        Aberto = 1,
        Cancelado = 2,
        Encerrado = 3,
        Modificacao = 4
    }

    public class TP_StatusOccurence
    {
        public int Id { get; set; }

        [Display(Name = "Status")]
        public string? Name { get; set; }
    }
    public enum TP_StatusOccurenceEnum : int
    {
        Procedente = 1,
        Nao_Procedente = 2,
        Nao_Identificao = 3,
        EmAnalise = 4
    }

}
