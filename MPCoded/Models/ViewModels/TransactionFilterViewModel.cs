using System.Transactions;
using Microsoft.AspNetCore.Mvc.Rendering;

public class TransactionFilterViewModel
{
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Type { get; set; }
    public List<SelectListItem> TransactionTypes { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
}
