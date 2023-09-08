using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.MVC.Hubs;
using RtChat.BusinessLayer.Services;
using RtChat.DataAccessLayer.Concrete;
using RtChat.EntityLayer.Concrete;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MessageServices>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;
	options.LoginPath = "/Login/Index";
	options.AccessDeniedPath = "/Home/AccessDenied";
	options.ReturnUrlParameter = "returnUrl";
	options.SlidingExpiration = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
	options.LogoutPath = "/Logout/Index";
	options.Cookie.SameSite = SameSiteMode.Lax;
	options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	options.Events.OnRedirectToLogin = context =>
	{
		if (context.Request.Path.StartsWithSegments("/api") &&
			context.Response.StatusCode == (int)HttpStatusCode.OK)
		{
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		}
		else
		{
			context.Response.Redirect(context.RedirectUri);
		}
		return Task.CompletedTask;
	};
});

builder.Services.AddScoped<MessageServices>();
builder.Services.AddDbContext<Context>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});
builder.Services.AddIdentity<User, AppRole>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
	options.Password.RequiredLength = 8;

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<Context>();

builder.Services.AddMvc(config =>
{
	var policy = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.Build();
	config.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chathub");


app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Main}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "login",
	pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
