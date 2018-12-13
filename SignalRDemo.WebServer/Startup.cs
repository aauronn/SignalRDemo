using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SignalRDemo.WebServer.Entities;
using SignalRDemo.WebServer.Hubs;
using SignalRDemo.WebServer.Services;

namespace SignalRDemo.WebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Old way to access connection string
            //var connectionString = Configuration["ConnectionStrings:CityInfoDB"];

            //New way to access ConnectionString
            var connectionString = Configuration.GetConnectionString("SRDemoContext");

            // Add DB Context
            services.AddDbContext<SRDemoContext>(o => o.UseSqlServer(connectionString));

            // Add identity Service
            services.AddIdentity<Principal, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<SRDemoContext>()
              .AddDefaultTokenProviders();

            // Add authentication 
            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //.AddCookie()
            .AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    //ValidIssuer = Configuration["Tokens:Issuer"],
                    //ValidAudience = Configuration["Tokens:Audiance"],
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                };

                // We have to hook the OnMessageReceived event in order to
                // allow the JWT authentication handler to read the access
                // token from the query string when a WebSocket or 
                // Server-Sent Events request comes in.
                cfg.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];


                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chatHub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Define CORS Policy
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyHeader()
                       //.WithOrigins("http://localhost:*")
                       .AllowAnyOrigin()
                       .AllowCredentials();
            }));

            
            // Redis connection string
            var redisConnectionString = Configuration["ConnectionStrings:REDIS_CONNECTIONSTRING"]; ;

            // Add SignalR
            services.AddSignalR()                       // Add SignalR to services
                    .AddMessagePackProtocol()           // Add MessagePack Protocol to SignalR
                    .AddRedis(redisConnectionString)    // Add Redis backplane
                    ;

            // Add MVC Service
            services.AddMvc(opt =>
            {
                //if (Env.IsProduction())
                //{
                //    opt.Filters.Add(new RequireHttpsAttribute());
                //}
            })
            .AddMvcOptions(o =>
                o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter())      // Add XML support as response
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            // Add User Provider for SignalR - This allow us to return user's identity from user's connection
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // Add CityInfo Seeder
            services.AddTransient<SRDemoSeeder>();

            // Repositories
            services.AddScoped<IActiveUserRepository, ActiveUserRepository>();
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Enable Authentication, this service was defined in ConfigureServices Method
            app.UseAuthentication();

            // Seed if development
            if (env.IsDevelopment())
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<SRDemoSeeder>();

                    seeder.Seed().Wait();
                }
            }

            // Enable CORS
            app.UseCors("CorsPolicy");

            // Define Hub Routes
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chathub");
            });

            // Define MVC Routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
