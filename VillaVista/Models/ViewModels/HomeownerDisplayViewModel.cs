//AddHomeownerViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace VillaVista.Models.ViewModels
{
    public class HomeownerDisplayViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HouseNumber { get; set; }
        public DateTime MoveInDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}