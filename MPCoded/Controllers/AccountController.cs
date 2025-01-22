using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MPCoded.Models;
using MPCoded.Models.ViewModels;

namespace MPCoded.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        #region injected services 
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signinManager;
        public AccountController(SignInManager<ApplicationUser> _signinManager, UserManager<ApplicationUser> _usermanager)
        {
            userManager = _usermanager;
            signinManager = _signinManager;
        }
        #endregion


        #region users
        public IActionResult Index()
        {

                
            return View();
        }
        [AllowAnonymous]
        public IActionResult RegisterStep1()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegisterStep1(RegisterStep1ViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Email"] = model.Email;
                TempData["Password"] = model.Password;
                TempData["ConfirmPassword"] = model.ConfirmPassword;
                return RedirectToAction("RegisterStep2");
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult RegisterStep2()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterStep2(RegisterStep2ViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["Email"].ToString();
                var password = TempData["Password"].ToString();

                string imagePath = null;
                if (model.ProfilePicture != null)
                {
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var filePath = Path.Combine(uploadsDir, model.ProfilePicture.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }
                    imagePath = "/images/" + model.ProfilePicture.FileName;
                }

                ApplicationUser user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    Gender = (ApplicationUser.Genders)model.Gender,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    CivilID = model.CivilID,
                    Balance = 0m,
                    AccountNumber = ApplicationUser.GenerateAccountNumber(),
                    ProfilePicturePath = imagePath // Add profile picture path
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await signinManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }




        [AllowAnonymous]


        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signinManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);




                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }


                ModelState.AddModelError("", "Invalid Email or Password");
            }
            return View(model);
        }


        public IActionResult Logout(string? ghjk)
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signinManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        #endregion



        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AccountDetails()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AccountDetailsViewModel
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





        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTransaction(TransactionViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                var accountViewModel = new AccountViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Balance = user.Balance,
                    ProfilePicturePath = user.ProfilePicturePath
                };

                return View("Index", accountViewModel);
            }

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Transaction.TransactionType type = (Transaction.TransactionType)Enum.Parse(typeof(Transaction.TransactionType), model.TransactionType);

            if (type == Transaction.TransactionType.Withdrawal && user.Balance < model.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance");
                return View("Index", new AccountViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Balance = user.Balance,
                    ProfilePicturePath = user.ProfilePicturePath
                });
            }

            var transaction = new Transaction
            {
                Amount = model.Amount,
                TransactionDate = DateTime.Now,
                Type = type,
                Description = string.IsNullOrWhiteSpace(model.Description) ? "No description provided" : model.Description, // Ensure non-null description
                ApplicationUserId = user.Id
            };

            user.Transactions.Add(transaction);
            user.Balance += (type == Transaction.TransactionType.Deposit) ? model.Amount : -model.Amount;
            await userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> AllUsers()
        {
            var currentUser = await userManager.GetUserAsync(User);

            var users = userManager.Users
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



        [Authorize]
        public async Task<IActionResult> Transfer(string id)
        {
            var targetUser = await userManager.FindByIdAsync(id);
            if (targetUser == null)
            {
                return NotFound();
            }

            var model = new TransferViewModel
            {
                TargetUserId = targetUser.Id,
                TargetUserName = $"{targetUser.FirstName} {targetUser.LastName}"
            };

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                var targetUser = await userManager.FindByIdAsync(model.TargetUserId);

                if (user == null || targetUser == null)
                {
                    return NotFound();
                }

                if (user.Balance < model.Amount)
                {
                    ModelState.AddModelError("", "Insufficient balance");
                    return View(model);
                }

                // Deduct from sender and add to recipient
                user.Balance -= model.Amount;
                targetUser.Balance += model.Amount;

                // Create transaction for sender
                var senderTransaction = new Transaction
                {
                    Amount = model.Amount,
                    TransactionDate = DateTime.Now,
                    Type = Transaction.TransactionType.Transfer,
                    Description = $"Transfer to {targetUser.FirstName} {targetUser.LastName}",
                    ApplicationUserId = user.Id
                };

                // Create transaction for recipient
                var recipientTransaction = new Transaction
                {
                    Amount = model.Amount,
                    TransactionDate = DateTime.Now,
                    Type = Transaction.TransactionType.Transfer,
                    Description = $"Transfer from {user.FirstName} {user.LastName}",
                    ApplicationUserId = targetUser.Id
                };

                user.Transactions.Add(senderTransaction);
                targetUser.Transactions.Add(recipientTransaction);

                // Update both users in the database
                await userManager.UpdateAsync(user);
                await userManager.UpdateAsync(targetUser);

                // Redirect to home page after transfer
                return RedirectToAction("Index", "Home");
            }

            // If the model is not valid, redisplay the form with validation errors
            return View(model);
        }





        [Authorize]
        public async Task<IActionResult> Transactions()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var transactions = user.Transactions.Select(t => new TransactionViewModel
            {
                TransactionId = t.TransactionId,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                Type = t.Type.ToString(),
                Description = t.Description
            }).ToList();

            return View(transactions);
        }





    }
}
