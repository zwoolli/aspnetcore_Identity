using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using WebApp.Data;
using WebApp.Models;
using WebApp.Settings;
using WebApp.Services;

namespace WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            // TODO: look at the bottom of the "Custom Storage Providers" Microsoft article. It looks like their
            // DapperUsersTable doesn't take a connecton string as input, but it's injected somehow. The UserStore
            // and RoleStore classes do not take a connection string, but are injected with the DapperUsersTable 
            // Or UserRepository (in my case)
            //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-5.0
            services.AddScoped<Npgsql.NpgsqlConnection>(e => new Npgsql.NpgsqlConnection(connectionString));
            services.AddScoped<IUserStore<ApplicationUser>, UserStore>();
            services.AddScoped<IRoleStore<ApplicationRole>, RoleStore>();


            // services.AddScoped<IUserStore<ApplicationUser>>(x => new UserStore(connectionString));
            // services.AddScoped<IRoleStore<ApplicationRole>>(x => new RoleStore(connectionString));
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(opts => {
                opts.SignIn.RequireConfirmedEmail = true;
            });
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            // services.AddTransient<IMailService, BasicSMTPService>();
            services.AddTransient<IMailService, MailKitService>();

            services.AddRazorPages();
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
