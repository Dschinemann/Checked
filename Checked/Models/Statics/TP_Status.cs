using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Enums
{
    public enum TP_Status : int
    {
        Aberto = 1,
        Cancelado = 2,
        Encerrado = 3,
        Modificacao = 4,     
    }
    public enum TP_StatusOccurence: int
    {
        Procedente = 1,
        Nao_Procedente = 2,
        Nao_Identificao = 3,
        EmAnalise = 4
    }
}
