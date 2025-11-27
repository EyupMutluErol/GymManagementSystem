using GymManagementSystem.Entities.Concrete;
using GymManagementSystem.WebUI.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> CreateRoles()
        {
            // RoleManager'a ihtiyacımız var, Constructor'a eklemeliyiz ama şimdilik hızlı çözüm:
            var roleManager = HttpContext.RequestServices.GetService<RoleManager<IdentityRole<int>>>();

            string[] roleNames = { "Admin", "Trainer", "Member" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
            return Content("Roller başarıyla oluşturuldu.");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    // Varsayılan değerler
                    PhotoUrl = "default-user.png"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Şimdilik varsayılan olarak "Member" rolü atayalım (İleride RoleManager ile yapacağız)
                    // await _userManager.AddToRoleAsync(user, "Member");

                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        if (item.Code.Contains("Email") || item.Description.Contains("e-posta"))
                        {
                            // Email ile ilgili hatayı "Email" inputunun altına yaz
                            ModelState.AddModelError("Email", item.Description);
                        }
                        else if (item.Code.Contains("Password") || item.Description.Contains("Şifre"))
                        {
                            // Şifre ile ilgili hatayı "Password" inputunun altına yaz
                            ModelState.AddModelError("Password", item.Description);
                        }
                        else
                        {
                            // Diğer hataları genel olarak en üste yaz
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    // 2. Rolüne bak ve yönlendir
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin"); // Admin Paneline
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Trainer"))
                    {
                        return RedirectToAction("Index", "Trainer"); // (Henüz yok ama yapacağız)
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Normal Üye Ana Sayfaya
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı e-posta veya şifre.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
