using AppService.Api.Data;
using AppService.Api.Data.Service;
using AppService.Api.Data.Service.Identity;
using AppService.Models;
using AppService.Models.Contants;
using AppService.Models.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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


            AddIdentity(services); //Identity 재정의
            AddJwtAuthentication(services); // 인증처리를 세팅 함수
            services.AddTransient<IInitializer, UserDbInitializer>(); //마이그레이션을 위한 설정
            services.AddControllers(); //최초 Api Controller 사용을 위한 설정값            
            services.AddServerSideBlazor();//[2]여기 추가
            services.AddRazorPages();//[2]여기 추가
            
            //swagger 등록  
            services.AddSwaggerGen();//Swagger 추가
            //swagger 등록  


            services.AddTransient<IJwtGeneratorService, JwtGeneratorService>();//jwt를 위한 설정
            services.AddTransient<IIdentityRepository, IdentityRepository>(); //사용자인증 종속성 주입

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

        private void AddJwtAuthentication(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes("dsafa!sdfasdfsafdasdfasdfasfdasfsd");
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserRepository, CurrentUserRepository>();
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

            app.UseHttpsRedirection(); //기존
            app.UseBlazorFrameworkFiles();//[1]여기 추가            
            app.UseStaticFiles(); //[2]여기 추가
            app.UseRouting(); //기존
            app.UseAuthentication(); //기존
            app.UseAuthorization(); //인증처리를 위한 추가


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();//[2]여기 추가
                endpoints.MapControllers(); //기존
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
