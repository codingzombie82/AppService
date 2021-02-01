using AppService.Api.Data;
using AppService.Models;
using AppService.Models.Contants;
using AppService.Models.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppService.Api
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
            //DbContext 종속성 주입
            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlServer(
                      Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            //Identity 재정의
            AddIdentity(services);

            services.AddTransient<IInitializer, UserDbInitializer>();
            services.AddControllers();            
            services.AddServerSideBlazor();//[2]여기 추가
            services.AddRazorPages();//[2]여기 추가
            
            //swagger 등록  
            services.AddSwaggerGen();//Swagger 추가
                                     //swagger 등록  


        }

        private void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = IdentityConst.MinPasswordLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();
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
                app.UseHsts();
            }
            //swagger 등록  
            app.UseSwagger(); //swagger 등록                             
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            //swagger 등록 

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();//[1]여기 추가            
            app.UseStaticFiles(); //[2]여기 추가
            app.UseRouting();
            app.UseAuthentication();
   
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();//[2]여기 추가
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");//[1]여기 추가
            });


            /// DB 마이그레이션을 위한 데이터 처리
            using var serviceScope = app.ApplicationServices.CreateScope();
            var initializers = serviceScope.ServiceProvider.GetServices<IInitializer>();
            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }
            ///
        }
    }
}
