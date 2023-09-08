using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RtChat.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtChat.DataAccessLayer.Concrete
{
    public class Context : IdentityDbContext<User, AppRole, int>
    {
		public Context(DbContextOptions<Context> options) : base(options)
		{
		}

	}
}
