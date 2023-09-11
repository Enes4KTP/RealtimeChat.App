using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtChat.EntityLayer.Concrete
{
	public class User : IdentityUser<int>
	{
		public string? Name { get; set; }
		public string? Surname { get; set; }
		public string PhotoUrl { get; set; }
		public DateTime? lastSeen { get; set; }

	}
}
