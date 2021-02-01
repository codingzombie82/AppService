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
            //DbContext ���Ӽ� ����
            services.AddDbContext<ApplicationDbContext>(options =>
                  options.UseSqlServer(
                      Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            //Identity ������
            AddIdentity(services);

            services.AddTransient<IInitializer, UserDbInitializer>();
            services.AddControllers();            
            services.AddServerSideBlazor();//[2]���� �߰�
            services.AddRazorPages();//[2]���� �߰�
            
            //swagger ���  
            services.AddSwaggerGen();//Swagger �߰�
                                     //swagger ���  


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
            //swagger ���  
            app.UseSwagger(); //swagger ���                             
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            //swagger ��� 

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();//[1]���� �߰�            
            app.UseStaticFiles(); //[2]���� �߰�
            app.UseRouting();
            app.UseAuthentication();
   
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();//[2]���� �߰�
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");//[1]���� �߰�
            });


            /// DB ���̱׷��̼��� ���� ������ ó��
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
