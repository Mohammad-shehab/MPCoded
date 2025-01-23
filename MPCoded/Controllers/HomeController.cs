using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MPCoded.Models;
using MPCoded.Models.ViewModels;
using System.Threading.Tasks;

namespace MPCoded.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new HomeIndexViewModel
            {
                Account = new AccountViewModel
                {
                    Email = user.Email,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender.ToString(),
                    CivilID = user.CivilID,
                    Balance = user.Balance,
                    AccountNumber = user.AccountNumber,
                    ProfilePicturePath = user.ProfilePicturePath
                },
                Transaction = new TransactionViewModel
                {
                    ApplicationUserId = user.Id
                }
            };

            return View(model);
        }


        public async Task<IActionResult> AccountDetails()
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var model = new AccountViewModel
                {
                    Email = user.Email,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender.ToString(),
                    CivilID = user.CivilID,
                    Balance = user.Balance,
                    AccountNumber = user.AccountNumber,
                    ProfilePicturePath = user.ProfilePicturePath
                };

                return View(model);
            }

            public async Task<IActionResult> AllUsers()
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var users = _userManager.Users
                    .Where(user => user.Id != currentUser.Id)
                    .Select(user => new UserCardViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Balance = user.Balance,
                        ProfilePicturePath = user.ProfilePicturePath
                    }).ToList();

                return View(users);
            }



        [HttpGet]
        public async Task<IActionResult> EditAccountDetails()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AccountViewModel
            {
                Email = user.Email,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender.ToString(),
                CivilID = user.CivilID,
                Balance = user.Balance,
                AccountNumber = user.AccountNumber,
                ProfilePicturePath = user.ProfilePicturePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAccountDetails(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.Email = model.Email;
            user.UserName = model.Username;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender =  user.Gender;
            user.CivilID = model.CivilID;
            user.ProfilePicturePath = model.ProfilePicturePath;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("AccountDetails");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

    }

} 


