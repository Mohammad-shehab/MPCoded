﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPCoded.Data;
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

        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion




       





    }
}


    
