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

namespace RealtimeChat.MVC.Controllers
{
	[Authorize]
	public class MainController : Controller
	{
		private readonly MessageServices _messageServices;
		private readonly UserManager<User> _userManager;
		private readonly IHubContext<ChatHub> _hubContext;

		public MainController(MessageServices messageServices, UserManager<User> userManager, IHubContext<ChatHub> hubContext)
		{
			_messageServices = messageServices;
			_userManager = userManager;
			_hubContext = hubContext;
		}

		public async Task<IActionResult> Index(string userId)
		{
			User? currentUser = await _userManager.GetUserAsync(User);
			List<Messages> receivedMessages = await _messageServices.GetMessagesByRecipientAsync(currentUser.Id.ToString());
			List<Messages> currentUserMessages = await _messageServices.GetMessagesForUserAsync(currentUser.Id.ToString(), userId);

			var displayedUsers = new HashSet<string>(receivedMessages.Select(message => message.From).Distinct());
			displayedUsers.UnionWith(receivedMessages.Select(message => message.To));

			var users = await _userManager.Users.Select(user => new User
			{
				Id = user.Id,
				UserName = user.UserName,
				Name = user.Name,
				Surname = user.Surname,
				PhotoUrl = user.PhotoUrl

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

			Messages message = new Messages
			{
				From = currentUser.Id.ToString(),
				To = recipientUser.Id.ToString(),
				Message = model.Message,
				Time = DateTime.UtcNow,
				IsRead = false,
				User = currentUser
			};

			try
			{
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
	}
}