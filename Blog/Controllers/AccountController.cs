using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerUser)
        {
            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(registerUser.ImagePorfile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("FeatureImage", "Invalid image format. Allowed formats are: " + string.Join(", ", allowedExtensions));
                    
                    return View(registerUser);
                }
                string imageUrl = null;
                if (registerUser.ImagePorfile != null)
                {
                    imageUrl = await ProcessUploadedFile(registerUser.ImagePorfile);
                }
                var user = new AppUser
                {
                    UserName = registerUser.UserName,
                    Email = registerUser.Email,
                    ProfilePictureUrl = imageUrl
                };
                var result = await userManager.CreateAsync(user, registerUser.Password);
                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        var role = new IdentityRole("User");
                        await roleManager.CreateAsync(role);
                    }
                    await userManager.AddToRoleAsync(user, "User");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Post");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerUser);
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInViewModel logInUser)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(logInUser.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
                    return View(logInUser);
                }
                var result = await signInManager.PasswordSignInAsync(user, logInUser.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Post");
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(logInUser);
            }
            return View(logInUser);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Post");
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel registerUser)
        {
            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(registerUser.ImagePorfile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("FeatureImage", "Invalid image format. Allowed formats are: " + string.Join(", ", allowedExtensions));

                    return View(registerUser);
                }
                string imageUrl = null;
                if (registerUser.ImagePorfile != null)
                {
                    imageUrl = await ProcessUploadedFile(registerUser.ImagePorfile);
                }
                var user = new AppUser
                {
                    UserName = registerUser.UserName,
                    Email = registerUser.Email,
                    ProfilePictureUrl = imageUrl
                };
                var result = await userManager.CreateAsync(user, registerUser.Password);
                if (result.Succeeded)
                {
                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        var role = new IdentityRole("Admin");
                        await roleManager.CreateAsync(role);
                    }
                    await userManager.AddToRoleAsync(user, "Admin");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Post");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerUser);
        }

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var imagesFloderPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imagesFloderPath))
            {
                Directory.CreateDirectory(imagesFloderPath);
            }
            var filePath = Path.Combine(imagesFloderPath, fileName);
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                //log exception
                return "Error Uploading Image: " + ex.Message;
            }
            return fileName;
        }
    }
}
