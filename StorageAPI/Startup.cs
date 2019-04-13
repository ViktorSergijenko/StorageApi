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
            //"Server=tcp:warehouse-manager-dbserver.database.windows.net,1433;Initial Catalog=warehouse-manager-api_db;Persist Security Info=False;User ID=viktor@warehouse-manager-dbserver;Password=bRAVO1996;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
            services.AddDbContext<StorageContext>(
                opt => opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=WarehouseStorage;Trusted_Connection=True;"));
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddMvc()
                .AddJsonOptions(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOneOrigin",
                    builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            }
            );

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
            app.UseCors("AllowOneOrigin");
        }
    }
}
