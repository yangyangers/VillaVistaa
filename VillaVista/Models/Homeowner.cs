using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaVista.Models
{
    public class Homeowner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public string? HouseNumber { get; set; }

        public DateTime MoveInDate { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }
    }
}