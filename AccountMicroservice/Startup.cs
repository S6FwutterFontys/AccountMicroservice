using System.Text;
using AccountMicroservice.DatastoreSettings;
using AccountMicroservice.Helpers;
// using AccountMicroservice.Messaging;
using AccountMicroservice.Repositories;
using AccountMicroservice.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AccountMicroservice
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
            services.AddTransient<IRegexHelper, RegexHelper>();
            
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretJWT);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    };
                });
            
            services.AddCors();
            
            services.AddTransient<IHasher, Hasher>();

            
            
            services.AddTransient<IAccountService, AccountService>();
            
            services.AddTransient<IAccountRepository, AccountRepository>();
            
            services.Configure<AccountDatabaseSettings>(
                Configuration.GetSection(nameof(AccountDatabaseSettings)));

            services.AddSingleton<IAccountDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<AccountDatabaseSettings>>().Value);

            services.AddTransient<IJWTTokenGenerator, JWTTokenGenerator>();

            // services.AddMessagePublisher();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}