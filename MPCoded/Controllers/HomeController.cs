using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MPCoded.Models;
using MPCoded.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MPCoded.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signinManager;

        public HomeController(SignInManager<ApplicationUser> _signinManager, UserManager<ApplicationUser> _usermanager)
        {
            userManager = _usermanager;
            signinManager = _signinManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AccountViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Balance = user.Balance,
                ProfilePicturePath = user.ProfilePicturePath
            };

            return View(model);
        }



    }
}
