using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.MVC.Dtos.RegisterDto;
using RtChat.EntityLayer.Concrete;

namespace RealtimeChat.MVC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;

        public RegisterController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(CreateNewUserDto createNewUserDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var appUser = new User()
            {
                Name = createNewUserDto.Name,
                Surname = createNewUserDto.Surname,
                Email = createNewUserDto.Mail,
                UserName = createNewUserDto.Username
            };
            var result = await _userManager.CreateAsync(appUser, createNewUserDto.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index","Login");
            }
            return View();
        }
    }
}
