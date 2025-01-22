using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MPCoded.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Select Gender")]
        public Genders Gender { get; set; }
        public enum Genders { Male, Female }

        public decimal Balance { get; set; } = 0m; // Default to 0

        // Property for account number as an integer
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Account number must be a positive integer.")]
        public int AccountNumber { get; set; } = GenerateAccountNumber();

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        // Method to generate a unique account number
        public static int GenerateAccountNumber()
        {
            var random = new Random();
            return random.Next(1, int.MaxValue);
        }

        [Required(ErrorMessage = "Enter First Name")]
        [MinLength(1)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter Last Name")]
        [MinLength(1)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Enter Civil ID")]
        [MinLength(12)]
        [MaxLength(12)]
        public string CivilID { get; set; }

        public string ProfilePicturePath { get; set; }
    }
}
