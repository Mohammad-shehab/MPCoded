namespace MPCoded.Models.ViewModels
{
    public class AccountDetailsViewModel
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string CivilID { get; set; }
        public decimal Balance { get; set; }
        public int AccountNumber { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
