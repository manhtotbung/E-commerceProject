using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using WebBH.Areas.Data;
using WebBH.Models;
using WebBH.Respositories;

namespace WebBH.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ICartRepo _cartRepo;
        // ✅ Chỉ 1 constructor
        public LoginModel(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            ILogger<LoginModel> logger,
            ICartRepo cartRepo
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _cartRepo = cartRepo;
        }

        [BindProperty] public InputModel Input { get; set; } = new();

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
        public string? ReturnUrl { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required, EmailAddress] public string Email { get; set; } = string.Empty;
            [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
            [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                ModelState.AddModelError(string.Empty, ErrorMessage);

            ReturnUrl = returnUrl ?? Url.Content("~/");

            // Đảm bảo login sạch sẽ
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid) return Page();

            var result = await _signInManager.PasswordSignInAsync(
                userName: Input.Email,        // nếu đăng nhập bằng Email như Username
                password: Input.Password,
                isPersistent: Input.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                // ✅ Merge giỏ hàng session -> DB theo user
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                    await _cartRepo.MergeSessionToDbAsync(HttpContext, user.Id );

                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
