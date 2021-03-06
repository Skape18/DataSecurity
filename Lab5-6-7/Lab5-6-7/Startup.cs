using Lab5_6_7.Data;
using Lab5_6_7.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;
using Lab5_6_7.Options;
using Microsoft.Extensions.Azure;

namespace Lab5_6_7
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
            // Register hasher before identity
            services.AddTransient<IPasswordHasher<IdentityUser>, BcryptPasswordHasher<IdentityUser>>();
            services.AddScoped<ISensitiveDataService, SensitiveDataService>();
            services.AddScoped<IEncryptionKeyProvider, EncryptionKeyProvider>();

            services.AddAzureClients(builder =>
            {
                builder.AddSecretClient(new Uri("https://DataSecurityLab5.vault.azure.net"));

                builder.UseCredential(new DefaultAzureCredential());
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 3;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddPasswordValidator<MaxLengthPasswordValidator<IdentityUser>>()
                .AddPasswordValidator<Top100PasswordValidator<IdentityUser>>();

            services
                .AddOptions<CustomPasswordHasherOptions>()
                .Bind(Configuration.GetSection(CustomPasswordHasherOptions.SectionName))
                .Validate(options => PasswordHasherVersion.Versions.Contains(options.Version));

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
