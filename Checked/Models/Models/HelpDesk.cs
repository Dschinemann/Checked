using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Models
{
    public class HelpDesk
    {
        public int Id { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string UserId { get; set; }

        [Required]
        [Display(Name ="Url que retornou erro")]
        public string UrlAccess { get; set; }

        [Required]
        [Display(Name = "Mensagem de erro")]
        public string ErrorMessage { get;set; }

        [Required]
        [Display(Name = "Comentário")]
        public string? Message { get; set; }

        [Display(Name = "Resposta")]
        public string? Response { get;set;}

        public ApplicationUser CreatedBy { get; set; }
        public virtual string CreatedById { get; set; }

    }
}
