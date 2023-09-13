using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RtChat.BusinessLayer.Services;
using RtChat.EntityLayer.Concrete;
using RealtimeChat.MVC.Hubs;
using RealtimeChat.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Net.Http.Headers;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Text;

namespace RealtimeChat.MVC.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        private readonly MessageServices _messageServices;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public MediaTypeHeaderValue? MediaTypeHeader { get; private set; }

        public MainController(MessageServices messageServices, UserManager<User> userManager, IHubContext<ChatHub> hubContext, IWebHostEnvironment webHostEnvironment)
        {
            _messageServices = messageServices;
            _userManager = userManager;
            _hubContext = hubContext;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string userId)
        {
            User? currentUser = await _userManager.GetUserAsync(User);

			currentUser.lastSeen = DateTime.Now;
			await _userManager.UpdateAsync(currentUser);

			var receivedMessages = await _messageServices.GetMessagesByRecipientAsync(currentUser.Id.ToString());

			var displayedUsers = new HashSet<string>(receivedMessages.Select(message => message.From).Distinct());
            displayedUsers.UnionWith(receivedMessages.Select(message => message.To));

            var users = await _userManager.Users.Select(user => new User
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                PhotoUrl = user.PhotoUrl,
                lastSeen = user.lastSeen

            }).ToListAsync();

            MessagesViewModel model = new MessagesViewModel
            {
                ReceivedMessages = receivedMessages,
                DisplayedUsers = displayedUsers,
                CurrentUser = currentUser,
                Users = users
            };


            return View(model);
        }

		[HttpPost]
		public async Task<IActionResult> SendMessage([FromBody] Messages model)
		{
			User? currentUser = await _userManager.GetUserAsync(User);
			User? recipientUser = await _userManager.FindByIdAsync(model.To);

			try
			{
				var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 256);
				var keyGenerator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
				keyGenerator.Init(keyGenerationParameters);
				var keyPair = keyGenerator.GenerateKeyPair();
				var privateKeyParameters = (ECPrivateKeyParameters)keyPair.Private;

				// Şifreleme motorunu başlatın (AES kullanacağız)
				var cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7Padding");

				// Şifreleme anahtarını ayarlayın
				var key = new KeyParameter(privateKeyParameters.D.ToByteArrayUnsigned());

				// Mesajı şifrele
				cipher.Init(true, key);
				var inputBytes = Encoding.UTF8.GetBytes(model.Message);
				var encryptedBytes = cipher.DoFinal(inputBytes);

				// Şifrelenmiş mesajı Base64 ile kodlayın (kodlama işlemi tercihinize bağlıdır)
				var encryptedMessage = Convert.ToBase64String(encryptedBytes);

				Messages message = new Messages
				{
					From = currentUser.Id.ToString(),
					To = recipientUser.Id.ToString(),
					Message = encryptedMessage,
					Time = DateTime.UtcNow,
					IsRead = false,
					User = currentUser
				};

				await _messageServices.CreateAsync(message);

				await _hubContext.Clients.User(recipientUser.Id.ToString()).SendAsync("receiveMessage", new
				{
					message = message.Message,
					time = message.Time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
					isMine = false
				});

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				Console.WriteLine("Hata Mesajı: " + ex.Message);
				Console.WriteLine("Hata Türü: " + ex.GetType().FullName);
				return Json(new { success = false, error = ex.Message });
			}
		}

		public string DecryptMessage(string encryptedMessage, MessagesViewModel viewModel)
		{
			try
			{
				// Şifreleme motorunu başlatın (AES kullanılıyor)
				var cipher = CipherUtilities.GetCipher("AES/ECB/PKCS7Padding");

				// Şifreleme anahtarını ayarlayın (alıcının özel anahtarı)
				var privateKeyBytes = Convert.FromBase64String(viewModel.RecipientPrivateKeyBase64);
				var key = new KeyParameter(privateKeyBytes);

				// Şifreli mesajı Base64 ile kodlamadan önce byte dizisine dönüştürün
				var encryptedBytes = Convert.FromBase64String(encryptedMessage);

				// Şifreleme motorunu başlatın ve çözme işlemini gerçekleştirin
				cipher.Init(false, key);
				var decryptedBytes = cipher.DoFinal(encryptedBytes);

				// Çözülen metni UTF-8 kodlamasına göre çözün
				var decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);

				return decryptedMessage;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Hata Mesajı: " + ex.Message);
				Console.WriteLine("Hata Türü: " + ex.GetType().FullName);
				return null;
			}
		}


		public async Task<IActionResult> GetAllMessagesWithUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                List<Messages> userMessages = await _messageServices.GetMessagesForUserAsync(userId, currentUser.Id.ToString());

                return Json(new { success = true, messages = userMessages });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkMessagesAsRead(string userId)
        {
			// Kullanıcının mesajlarını işaretle
			await _messageServices.MarkMessagesAsRead(userId);

			await _hubContext.Clients.All.SendAsync("MessageMarkedAsRead", userId);

			return Json(new { success = true });
		}

		[HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return Json(new { success = false, error = "Dosya seçilmedi veya boş." });
            }

            try
            {
                var uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "img");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var currentUser = await _userManager.GetUserAsync(User);
                currentUser.PhotoUrl = "/img/" + uniqueFileName;
                await _userManager.UpdateAsync(currentUser);

                return Json(new { success = true, photoUrl = currentUser.PhotoUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}