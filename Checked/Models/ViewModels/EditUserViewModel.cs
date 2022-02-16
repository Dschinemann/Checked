using System.ComponentModel.DataAnnotations;
using Checked.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Checked.Models.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public virtual string? Id { get; set; }

        [Required]
        [Display(Name ="Nome")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "País")]
        public int CountryId { get; set; }

        [Display(Name = "País")]
        public Country? Country { get; set; }

        [Required]
        [Display(Name ="Estado")]
        public virtual int StateId { get; set; }

        public virtual List<State>? States { get; set; }

        [Display(Name = "Estado")]
        public virtual State? State { get; set; }


        public List<City>? Cities { get; set; }

        [Display(Name = "Cidade")]
        public City? City { get; set; }

        [Required]
        [Display(Name ="Cidade")]
        public int CityId { get; set; }

        [Required]
        [Display(Name ="CEP")]
        public string PostalCode { get; set; }
    }
}
