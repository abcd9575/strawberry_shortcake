using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Strawberry_Shortcake.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Strawberry_Shortcake
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddSession(); // 서비스에 등록함

            services.AddDbContext<Strawberry_ShortcakeContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Strawberry_ShortcakeContext")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => {
                    o.LoginPath = new PathString("/Users/Login");
                    o.LogoutPath = new PathString("/Users/Logout");
                    o.ExpireTimeSpan = new TimeSpan(0, 30, 0);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Strawberry_ShortcakeContext db)
        {
            if (env.IsDevelopment())
            {
                /* 개발중일 땐 모든 DB를 날리고 다시 생성 */
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (db.Database.GetPendingMigrations().Count() > 0) {
                    /* 
                     * 만약 Add-Migration한 내용들 중에 Update-Database하지 않은 게 있다면,
                     * 자동으로 Update-Database 호출
                     */
                    db.Database.Migrate();
                }

                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession(); // application 에서 사용하겠음.

            app.UseRouting();

            app.UseAuthentication();
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
