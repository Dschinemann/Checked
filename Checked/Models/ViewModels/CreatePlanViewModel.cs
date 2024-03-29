﻿using Checked.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.ViewModels
{
    public class CreatePlanViewModel
    {
        public string? PlanId { get; set; }
        [Required]
        [Display(Name ="Motivo")]
        public string Subject { get; set; } 
        [Required]
        [Display(Name = "Responsável")]
        public string AccountableId { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Previsão para finalizar o plano")]
        public DateTime Goal { get; set; } = DateTime.Now;
        [Required]
        [Display(Name = "Objetivo")]
        public string Objective { get; set; } 
        [Required]
        public string OccurrenceId { get; set; } 
        public string? OrganizatioId { get; set; }

        public SelectList ListUser;

        public string? CreateById { get; set; }

    }
}
