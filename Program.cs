using AutoMapper;
using ChatApp.Areas.Identity.Data;
using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Middlewares;
using ChatApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;

namespace ChatApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            string inquiryConnectionString = builder.Configuration.GetConnectionString("InquiryConnection") ?? throw new InvalidOperationException("Connection string 'InquiryConnection' not found.");
            string chatAppConnectionString = builder.Configuration.GetConnectionString("ChatAppConnection") ?? throw new InvalidOperationException("Connection string 'ChatAppConnection' not found.");

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDbContext<InquiryDbCtx>(options =>
                options.UseSqlServer(inquiryConnectionString));

            builder.Services.AddDbContext<ChatAppDbCtx>(options => 
                options.UseSqlServer(chatAppConnectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            // Mapper
			var mapperConfig = new MapperConfiguration(config =>
			{
				config.AddProfile(new MappingProfile());
			});

			IMapper mapper = mapperConfig.CreateMapper();
			builder.Services.AddSingleton(mapper);

            // Role/Policies
            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("RequireChatMod", policy => policy.RequireRole("ChatModerator"));
            });

            // SignalR
			builder.Services.AddSignalR();

			// Http
			builder.Services.AddHttpClient();

            // Chat service
			builder.Services.AddScoped<ChatService>();
			builder.Services.AddScoped<UserService>();
			builder.Services.AddScoped<ChatDataService>();

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

			app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            // Monitor request count using a middleware
            app.UseMiddleware<RequestCountMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.MapRazorPages();
			app.MapHub<ChatHub>("/chatHub");

			app.Run();
        }
    }
}