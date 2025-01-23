using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPCoded.Data;
using MPCoded.Models;
using MPCoded.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MPCoded.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public TransactionController(UserManager<ApplicationUser> _userManager, ApplicationDbContext context)
        {
            userManager = _userManager;
            _context = context;
        }



        public async Task<IActionResult> TransactionsList(decimal? minAmount, decimal? maxAmount, DateTime? startDate, DateTime? endDate)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var transactionsQuery = _context.Transactions
                .Where(t => t.ApplicationUserId == user.Id)
                .AsQueryable();

            if (minAmount.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Amount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Amount <= maxAmount.Value);
            }

            if (startDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.TransactionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                transactionsQuery = transactionsQuery.Where(t => t.TransactionDate <= endDate.Value);
            }

           

            var transactions = await transactionsQuery
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return View(transactions);
        }

     

        [HttpPost]
        public async Task<IActionResult> AddTransaction(HomeIndexViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Log ModelState errors
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                var accountViewModel = new HomeIndexViewModel
                {
                    Account = new AccountViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Balance = user.Balance,
                        ProfilePicturePath = user.ProfilePicturePath
                    },
                    Transaction = model.Transaction
                };

                return RedirectToAction("Index", "Home", accountViewModel);
            }

            if (!Enum.TryParse(model.Transaction.TransactionType, out Transaction.TransactionType type))
            {
                ModelState.AddModelError("", "Invalid transaction type.");

                var accountViewModel = new HomeIndexViewModel
                {
                    Account = new AccountViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Balance = user.Balance,
                        ProfilePicturePath = user.ProfilePicturePath
                    },
                    Transaction = model.Transaction
                };

                return RedirectToAction("Index", "Home", accountViewModel);
            }

            if (type == Transaction.TransactionType.Withdrawal && user.Balance < model.Transaction.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance.");

                var accountViewModel = new HomeIndexViewModel
                {
                    Account = new AccountViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Balance = user.Balance,
                        ProfilePicturePath = user.ProfilePicturePath
                    },
                    Transaction = model.Transaction
                };

                return RedirectToAction("Index", "Home", accountViewModel);
            }

            var transaction = new Transaction
            {
                Amount = model.Transaction.Amount,
                TransactionDate = DateTime.Now,
                Type = type,
                Description = string.IsNullOrWhiteSpace(model.Transaction.Description) ? "No description provided" : model.Transaction.Description,
                ApplicationUserId = user.Id
            };

            _context.Transactions.Add(transaction);

            user.Balance += (type == Transaction.TransactionType.Deposit) ? model.Transaction.Amount : -model.Transaction.Amount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error saving transaction: {ex.Message}");
            }

            return RedirectToAction("Index", "Home");
        }


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

    }
}

