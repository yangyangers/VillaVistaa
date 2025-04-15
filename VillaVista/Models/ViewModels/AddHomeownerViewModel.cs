//AddHomeownerViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace VillaVista.Models.ViewModels
{
    public class AddHomeownerViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Unit number is required")]
        public string UnitNumber { get; set; }

        [Required(ErrorMessage = "Move-in date is required")]
        [DataType(DataType.Date)]
        public DateTime MoveInDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public string Notes { get; set; }
        
    }
}