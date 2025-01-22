namespace MPCoded.Models.ViewModels
{
    public class AccountViewModel : TransactionViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
