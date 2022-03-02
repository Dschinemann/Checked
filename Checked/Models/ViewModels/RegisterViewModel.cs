using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class RegisterViewModel
    {

        public virtual string? Id { get; set; }
        public string? organizationId { get; set; }

        [Required]
        [Display(Name ="Nome")]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [Display(Name="Email")]
        public string Email { get; set; }
     
        [Required]
        [Display(Name ="País")]
        public int CountryId { get; set; }

        [Required]
        [Display(Name ="Estado")]
        public int StateId { get; set; }
        public string? StateName { get; set; }

        [Required]
        [Display(Name ="Cidade")]
        public int CityId{ get; set; }
        public string? CityName { get; set; }

        [Required]
        [Display(Name ="CEP")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
