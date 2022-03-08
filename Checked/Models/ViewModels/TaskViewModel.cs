namespace Checked.Models.ViewModels
{
    public class TaskViewModel
    {
        public int OcProcedente { get; set; }
        public int OcNaoIdentificado { get; set; }
        public int OcEmAnalise { get; set; }
        public int OcNãoProcedente { get; set; }

        public int PlEmTempo { get; set; }
        public int PlProxVencimento { get; set; }
        public int PlAtrasado { get; set; }
        public int PlFinalizado { get; set; }

        public int AcEmTempo { get; set; }
        public int AcProxVencimento { get; set; }
        public int AcAtrasado { get; set; }
        public int AcFinalizado { get; set; }

    }
}
