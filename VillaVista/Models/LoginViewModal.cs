﻿using System.ComponentModel.DataAnnotations;

namespace VillaVista.Models
{
    public class LoginViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

}
