namespace Checked.Models.Statics
{
    public class TP_TaskStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum TP_TaskStatusEnum : int
    {
        Start = 1,
        Pendente = 2,
        Finalizado = 3
    }
}
