using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.Data;
using Dashboard.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dashboard.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Read appsettings.json
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Init DbContext
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<Seed>();
            // Add Repository Interface Services (Scoped: instanced once per request)
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ISettingsRepository, SettingsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWeatherRepository, WeatherRepository>();
            services.AddScoped<IKanbanRepository, KanbanRepository>();
            // Use MVC 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                // Ignore JSON self-referencing
                .AddJsonOptions(
                    opt =>
                    {
                        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });
            // Enable cross-origin resource sharing (CORS)
            services.AddCors();
            // Add automapper
            services.AddAutoMapper();
            // Add and define Auth middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Use global exception handler
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts(); // Prevent HTTP, force HTTPS
            }

            // Force HTTPS
            //app.UseHttpsRedirection();

            // Seed DB
            seeder.SeedDB();

            // Enable cross-origin resource sharing (CORS)
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // Enable Auth
            app.UseAuthentication();

            // Enable serving Angular (from wwwroot)
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc(
                routes => {
                    routes.MapSpaFallbackRoute(
                        name: "spa-fallback",
                        defaults: new {controller = "Fallback", action = "index"}
                    );
                }
            );
        }
    }
}
