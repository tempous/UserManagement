using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index() => View(userManager.Users.ToList());

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    Name = model.Name,
                    UserName = model.UserName,
                    Email = model.Email,
                    RegistrationDate = DateTime.Now,
                    LastLogin = DateTime.Now,
                    Status = "Active"
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return Redirect("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) => View(new Login { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.UserName);

                    if (user != null)
                    {
                        user.LastLogin = DateTime.Now;
                        await userManager.UpdateAsync(user);
                    }

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);
                    else
                        return Redirect("Index");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string[] selectedUserId)
        {
            var action = "";

            if (selectedUserId != null)
            {
                foreach (var userId in selectedUserId)
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        user.Status = "Active";
                        user.LockoutEnabled = false;
                        user.LockoutEnd = DateTime.Now;

                        await userManager.UpdateAsync(user);
                    }
                }

                action = "Index";
            }

            return Redirect(action);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string[] selectedUserId)
        {
            var action = "Index";

            if (selectedUserId != null)
            {
                var isCurrentUserSelected = false;

                foreach (var userId in selectedUserId)
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        user.Status = "Blocked";
                        user.LockoutEnabled = true;
                        user.LockoutEnd = DateTime.Now.AddMinutes(30);

                        await userManager.UpdateAsync(user);

                        if (User.Identity.Name == user.UserName)
                            isCurrentUserSelected = true;
                    }
                }

                if (isCurrentUserSelected) await signInManager.SignOutAsync();
                //action = isCurrentUserSelected ? "Login" : "Index";
            }

            return Redirect(action);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string[] selectedUserId)
        {
            var action = "Index";

            if (selectedUserId != null)
            {
                var isCurrentUserSelected = false;

                foreach (var userId in selectedUserId)
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        user.LockoutEnabled = true;
                        user.LockoutEnd = DateTime.Now;

                        if (User.Identity.Name == user.UserName)
                            isCurrentUserSelected = true;

                        await userManager.DeleteAsync(user);
                    }
                }

                if (isCurrentUserSelected) await signInManager.SignOutAsync();
                //action = isCurrentUserSelected ? "Login" : "Index";
            }

            return Redirect(action);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Redirect("Login");
        }
    }
}
