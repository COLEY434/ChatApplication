using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApplication.Database;
using ChatApplication.Hubs;
using ChatApplication.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ChatApplication
{
    public class Startup
    {
        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options =>
                      options.UseSqlServer(_configuration.GetConnectionString("ChatDatabase")));


            services.AddIdentity<User, IdentityRole>(options => 
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequiredLength = 6;
                       
                      
                    })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.AddSignalR();
            //used to redirect to a specific path after failed authorization
           // services.ConfigureApplicationCookie(options => options.LoginPath = "/login");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(opt =>
                {
                    opt.MapHub<ChatHub>("/chathub");
                    opt.MapDefaultControllerRoute();
                   
                }
            
            );
           
        }
    }
}
