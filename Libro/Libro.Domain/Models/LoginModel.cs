﻿using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
