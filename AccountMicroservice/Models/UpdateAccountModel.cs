﻿using System.ComponentModel.DataAnnotations;

 namespace AccountMicroservice.Models
{
    public class UpdateAccountModel
    {
        [Required]
        public string Email { get; set; }
    }
}