using System.ComponentModel.DataAnnotations;

namespace MPCoded.Models.ViewModels
{
    public class TransactionViewModel
    {
        [Required]
        public string TransactionType { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
            public int TransactionId { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Type { get; set; }
        }


    }

