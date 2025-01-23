using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MPCoded.Models.ViewModels;

namespace MPCoded.Models
{
    public class Transaction 
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now; // Set default value to current date and time

        [Required]
        public TransactionType Type { get; set; }
        public enum TransactionType { Transfer, Deposit, Withdrawal }



        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
