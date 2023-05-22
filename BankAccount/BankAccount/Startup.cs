using BankAccount.BllServices;
using BankAccount.DB;
using BankAccount.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace BankAccount
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IBankAccountDataAccessService, BankAccountDataAccessService>();
            services.AddTransient<IBllAccountService, BllAccountService>();
            var connectionString = Configuration.GetConnectionString("BankAccountDatabase");
            services.AddDbContext<BankAccountContext>(options =>
            {
                options.UseSqlite(connectionString, x => { x.MigrationsAssembly("BankAccount");});
                
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BankAccountContext bankAccountContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //bankAccountContext.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}


