using System.ComponentModel.DataAnnotations;

namespace VillaVista.Models.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
        public string Role { get; internal set; }
        public string FullName { get; internal set; }

        public string? Position { get; set; }
        public string? HouseNumber { get; set; }
    }




}
