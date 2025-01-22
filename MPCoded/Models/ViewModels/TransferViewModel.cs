using System.ComponentModel.DataAnnotations;

public class TransferViewModel
{
    public string TargetUserId { get; set; }
    public string TargetUserName { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}
