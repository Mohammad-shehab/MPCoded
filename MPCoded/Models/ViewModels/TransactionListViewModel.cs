using System.Transactions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MPCoded.Models.ViewModels
{
    public class TransactionsListViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Type { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
