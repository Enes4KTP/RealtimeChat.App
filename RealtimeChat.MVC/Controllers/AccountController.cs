using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.MVC.Dtos.LoginDto;
using RtChat.EntityLayer.Concrete;
using System.Threading.Tasks;

namespace RealtimeChat.MVC.Controllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly SignInManager<User> _signInManager;

		public AccountController(SignInManager<User> signInManager)
		{
			_signInManager = signInManager;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken] // Add anti-forgery token validation
		public async Task<IActionResult> Login(LoginUserDto loginUserDto)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(loginUserDto.Username, loginUserDto.Password, false, lockoutOnFailure: true);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Main");
				}
				if (result.IsLockedOut) // User is locked out due to too many attempts
				{
					ModelState.AddModelError("", "Account is locked. Please try again later.");
				}
				else
				{
					ModelState.AddModelError("", "Invalid login attempt. Please check your credentials.");
				}
			}

			return View();
		}
	}
}
