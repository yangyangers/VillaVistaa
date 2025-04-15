using System;
using System.ComponentModel.DataAnnotations;

namespace VillaVista.Models.ViewModels
{
    public class EditHomeownerViewModel
    {
        public string Id { get; set; } // UserId
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UnitNumber { get; set; }
        public DateTime MoveInDate { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
