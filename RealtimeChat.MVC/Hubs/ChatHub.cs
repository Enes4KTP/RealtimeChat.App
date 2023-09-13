using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using RtChat.BusinessLayer.Services;
using RtChat.EntityLayer.Concrete;
using System;
using System.Threading.Tasks;


namespace RealtimeChat.MVC.Hubs
{
	public class ChatHub : Hub
	{
		private readonly MessageServices _messageServices;

		public ChatHub(MessageServices messageServices)
		{
			_messageServices = messageServices;
		}

		public async Task MarkMessageAsRead(string messageId)
		{
			// messageId ile ilgili mesajı işaretle
			await _messageServices.MarkMessagesAsRead(messageId);

			// İlgili kullanıcıya mesajın işaretlendiğini bildirin
			await Clients.User(Context.User.Identity.Name).SendAsync("MessageMarkedAsRead", messageId);
		}


		public async Task SendMessage(Messages message)
		{
			try
			{
				// Mesajı veritabanına kaydet
				await _messageServices.CreateAsync(message);

				var formattedTime = message.Time.ToString("HH:mm");

				// Alıcı kullanıcıya mesajı gönder
				await Clients.User(message.To).SendAsync("receiveMessage", new
				{
					message = message.Message,
					time = formattedTime,
					from = message.From
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Hata Mesajı: " + ex.Message);
				Console.WriteLine("Hata Türü: " + ex.GetType().FullName);
			}
		}
	}
}

//namespace RealtimeChat.MVC.Hubs
//{
//	public class ChatHub : Hub
//	{
//		private readonly MessageServices _messageServices;
//		private readonly UserManager<User> _userManager;

//		public ChatHub(MessageServices messageServices, UserManager<User> userManager)
//		{
//			_messageServices = messageServices;
//			_userManager = userManager;
//		}

//		public async Task SendMessage(Messages model, string recipientUserId)
//		{
//			try
//			{
//				var httpContext = Context.GetHttpContext();
//				var user = httpContext.User;

//				User currentUser = await _userManager.GetUserAsync(user);
//				User recipientUser = await _userManager.FindByIdAsync(model.To);

//				Messages message = new Messages
//				{
//					From = currentUser.Id.ToString(),
//					To = recipientUser.Id.ToString(),
//					Message = model.Message,
//					Time = DateTime.UtcNow,
//					IsRead = false,
//					User = currentUser
//				};

//				await _messageServices.CreateAsync(message);

//				var formattedTime = message.Time.ToString("HH:mm");

//				// Alıcı kullanıcıya mesajı gönder
//				await Clients.User(recipientUserId).SendAsync("receiveMessage", new
//				{
//					message = message.Message,
//					time = message.Time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
//					isMine = false
//				});
//			}
//			catch (Exception ex)
//			{
//				Console.WriteLine("Hata Mesajı: " + ex.Message);
//				Console.WriteLine("Hata Türü: " + ex.GetType().FullName);
//			}
//		}
//	}
//}
