using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CirAnime.Pages;
using Microsoft.Extensions.FileProviders;
using tusdotnet;
using tusdotnet.Models;
using tusdotnet.Stores;
using tusdotnet.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CirAnime.Data;
using System.Linq;
using System.IO;
using CirAnime.Models;
using CirAnime.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CirAnime
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
            services.AddHttpContextAccessor();
            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Video", "/v/{videoid}.json");
            });

            services.AddRouting();
            services.AddTransient<ICarService, CarService>();



            services.AddAuthentication(options =>
            {
          // If an authentication cookie is present, use it to get authentication information
          options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
          // If authentication is required, and no cookie is present, use Okta (configured below) to sign in
          options.DefaultChallengeScheme = "Discord";
            })
              .AddCookie() // cookie authentication middleware first
              .AddOAuth("Discord", options =>
              {

            // Oauth authentication middleware is second
            //var oktaDomain = Configuration.GetValue<string>("Okta:OktaDomain");
            // When a user needs to sign in, they will be redirected to the authorize endpoint
            //options.AuthorizationEndpoint = $"{oktaDomain}/oauth2/default/v1/authorize";
            options.AuthorizationEndpoint = "https://discordapp.com/api/oauth2/authorize";
            // Okta's OAuth server is OpenID compliant, so request the standard openid
            // scopes when redirecting to the authorization endpoint
            //options.Scope.Add("openid");
            //options.Scope.Add("profile");
            options.Scope.Add("identify");
            // After the user signs in, an authorization code will be sent to a callback
            // in this app. The OAuth middleware will intercept it
            options.CallbackPath = new PathString("/discord-login");
            // The OAuth middleware will send the ClientId, ClientSecret, and the
            // authorization code to the token endpoint, and get an access token in return
            options.ClientId = Configuration.GetValue<string>("Discord:AppId");
                  options.ClientSecret = Configuration.GetValue<string>("Discord:AppSecret");
                  options.TokenEndpoint = "https://discordapp.com/api/oauth2/token";
            // Below we call the userinfo endpoint to get information about the user
            options.UserInformationEndpoint = "https://discordapp.com/api/users/@me";
            // Describe how to map the user info we receive to user claims
            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                  options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");

            //options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email"); 
            options.Events = new OAuthEvents
                  {
                      OnCreatingTicket = async context =>
                {
                // Get user info from the userinfo endpoint and use it to populate user claims
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                      request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                      var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                      response.EnsureSuccessStatusCode();
                      var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                      context.RunClaimActions(user.RootElement);
                  }
                  };
              });

            var physicalProvider = new PhysicalFileProvider(Configuration.GetValue<string>("FileUploadPath"));
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddDbContext<CirAnimeContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("CirAnimeContext")));

            services.AddScoped<MyCustomTusConfiguration>();
            services.AddAuthorization(options =>
            {
                var allowedUsers = Configuration.GetValue<string[]>("Uploaders");
                options.AddPolicy("Hoster", policy =>
                {
                    var allowedUsers = Configuration.GetSection("Uploaders").Get<List<string>>();
                    policy.RequireClaim(ClaimTypes.NameIdentifier, allowedUsers);
                });

            });

        }


        private async Task ProcessFile(ITusFile file)
        {

            return;
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CirAnimeContext dbContext)
        {
            if (env.IsDevelopment())
            {
                var a = Configuration.GetValue<string>("MediaFilePath");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseTus(
              cont => cont.RequestServices.GetService<MyCustomTusConfiguration>().GetTusConfiguration()
              );

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Configuration.GetValue<string>("MediaFilePath")),
                RequestPath = "/media"// remove ?, , etc from url
            });
        }

    }
}
