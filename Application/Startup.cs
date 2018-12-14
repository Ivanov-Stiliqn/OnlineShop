using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Hubs;
using Application.Infrastructure.Extensions;
using Application.Infrastructure.Helpers;
using Application.Infrastructure.Mapping;
using Application.Infrastructure.Middlewares;
using Application.Models;
using AutoMapper;
using CloudinaryDotNet;
using Data;
using Data.Repositories;
using Data.Repositories.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDomainServices();
            services.AddAutoMapper(options => options.AddProfile<AutoMapperProfile>());
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });

            services.AddCloudinary(new Account(
                Configuration.GetSection("CloudinarySettings")["Name"],
                Configuration.GetSection("CloudinarySettings")["AppKey"],
                Configuration.GetSection("CloudinarySettings")["AppSecret"]
            ));

            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));

            services.AddMemoryCache();

            services.AddSignalR();

            services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            app.UseMiddleware<DatabaseSeedMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseSignalR(routes => routes.MapHub<ChatHub>("/chat"));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller}/{action}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
