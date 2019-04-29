using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StorageAPI.Context;
using StorageAPI.Services;
using AutoMapper;
using StorageAPI.Configs;
using StorageAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace StorageAPI
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
            //(@"Server=(localdb)\mssqllocaldb;Database=WarehouseStorage;Trusted_Connection=True;")
            //@"Server=tcp:warehouse-manager-dbserver.database.windows.net,1433;Initial Catalog=warehouse-manager-api_db;Persist Security Info=False;User ID=viktor@warehouse-manager-dbserver;Password=bRAVO1996;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            // Injecting AppSettings
            services.Configure<ApplicationSettings>(Configuration.GetSection(""));
            // Setting our connection string 
            services.AddDbContext<StorageContext>(
                opt => opt.UseSqlServer(Configuration.GetConnectionString("LocalConnection")));
            // Adding Identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<StorageContext>();
            // Adding automapper
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddMvc()
                .AddJsonOptions(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // Adding cors policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOneOrigin",
                    builder => builder
                    .WithOrigins(Configuration["ApplicationSettings:APP_URL"].ToString())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            }
            );
            // JWT Authentification
            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_KEY"].ToString());
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                };
            });

            #region Services injections
            services.AddSingleton<IConfiguration>(o => Configuration);
            services.AddScoped<StorageContext>();
            services.AddScoped<WarehouseService>();
            services.AddScoped<NewsService>();
            AutoMapperConfig.RegisterMappings(services.BuildServiceProvider());
            #endregion Services injections

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseAuthentication();
            app.UseCors("AllowOneOrigin");
        }
    }
}
