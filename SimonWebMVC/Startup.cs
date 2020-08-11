using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using CL3C.Services;
using ExchangeRatesLib;
using Microsoft.AspNetCore.Identity;
using CL3C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Http;
using SimonWebMVC.Security;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using reCAPTCHA.AspNetCore;

namespace SimonWebMVC
{
    public class Startup
    {
        private string _connection = null;
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("Cl3CDb"));
            // builder.Password = Configuration["DbPassword"];
            _connection = _config.GetConnectionString("CL3CDbConnectionString");

            services.AddDbContextPool<CL3CContext>(options => options.UseMySql(_connection));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Override password requiremets
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 4;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.SignIn.RequireConfirmedEmail = true;

                options.Tokens.PasswordResetTokenProvider = "CustomPasswordReset";
            })
            .AddEntityFrameworkStores<CL3CContext>()
            // Adds token provider for email confirmation, etc.
            .AddDefaultTokenProviders()
            //  Adds custom token provider
            .AddTokenProvider<CustomPasswordResetTokenProvider<ApplicationUser>>("CustomPasswordReset");

            // Changes token lifespan of password reset token type
            services.Configure<CustomPasswordResetTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromHours(1));

            // Adds "[Authorize] attribute to every controller within the project -> Every page will be accesible only after succesfull login unless specified otherwise"
            // To make page accesible to anonymous users add "[AllowAnonymous]" attribute to the fuction or controller
            services.AddMvc(options => {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddAuthorization(options =>
            {
                
            });

            services.AddScoped<ICarRepo, CarRepo>();
            services.AddSingleton<IExchangeRatesClient, ExchangeRatesClient>();

            services.AddScoped<IAuthorizationHandler, CarHandler>();
            services.AddScoped<IAuthorizationHandler, UserAdministrationHandler>();
            services.AddScoped<IAuthorizationHandler, RolesAdministrationHandler>();

            services.AddSingleton<DataProtectionPurposeStrings>();

            services.AddMailKit(config => config.UseMailKit(_config.GetSection("Email").Get<MailKitOptions>()));

            services.AddRecaptcha(_config.GetSection("RecaptchaSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseHsts();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
