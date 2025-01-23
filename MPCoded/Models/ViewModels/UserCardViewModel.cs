using System.ComponentModel.DataAnnotations;

namespace MPCoded.Models.ViewModels
{
    public class UserCardViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}