﻿﻿using System.ComponentModel.DataAnnotations;

 namespace AccountMicroservice.Models
{
    public class CreateAccountModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}