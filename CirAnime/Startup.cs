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
      services.AddRazorPages();
        
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
                options.UseSqlServer(Configuration.GetConnectionString("CirAnimeContext")));

      services.AddScoped<MyCustomTusConfiguration>();

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
        cont => cont.RequestServices.GetService<MyCustomTusConfiguration>()
        );
    //  app.UseTus(CreateTusConfiguration);

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
      });
    }

    /*
    private DefaultTusConfiguration ConfigureTus(HttpContext httpContext)
    {
      return new DefaultTusConfiguration
      {
        Store = new TusDiskStore(Configuration.GetValue<string>("FileUploadPath")),
        UrlPath = "/uploadTus",// + httpContext.User.Identity,
        
        Events = new tusdotnet.Models.Configuration.Events
        {
          OnFileCompleteAsync = async eventContext =>
          {
            ITusFile file = await eventContext.GetFileAsync();
            Console.WriteLine("file finished");
            await ProcessFile(file);
          },
          OnAuthorizeAsync = async eventContext =>
          {
            eventContext.FailRequest("not authorized");
          }
          
        }
      };

    } */

    public DefaultTusConfiguration CreateTusConfiguration(HttpContext httpContext)
    {

      if (!httpContext.User.Identity.IsAuthenticated)
      {
        return null;
      }

      string baseDirectory = Configuration.GetValue<string>("FileUploadPath");
      if (!baseDirectory.EndsWith("/")) baseDirectory.Append('/');


      string usersubdirectory = httpContext.User.Identity.Name;
      usersubdirectory = usersubdirectory.Replace('/', '_').Replace("..", "_");


      string uploadDirectory = baseDirectory + usersubdirectory + "/";
      if (!Directory.Exists(uploadDirectory))
      {
        Directory.CreateDirectory(uploadDirectory);
      }

      return new DefaultTusConfiguration
      {
        Store = new TusDiskStore(uploadDirectory),
        UrlPath = "/uploadTus",// + httpContext.User.Identity,

        Events = new tusdotnet.Models.Configuration.Events
        {

          OnFileCompleteAsync = async eventContext =>
          {
            ITusFile file = await eventContext.GetFileAsync();
            var metaData = await file.GetMetadataAsync(eventContext.CancellationToken);
            string filename = metaData["filename"].GetString(System.Text.Encoding.UTF8);
            
            UploadEntry entry = new UploadEntry
            {
              OriginalFileName = filename,
              Owner = httpContext.User.Identity.Name,
              Title = filename.Contains('.') ? filename.Remove(filename.LastIndexOf('.')) : filename,
              UploadDate = DateTime.Now
            };
           // dbContext.UploadEntry.Add(entry);

            Console.WriteLine("file finished");
            await ProcessFile(file);

          },
          OnAuthorizeAsync = async eventContext =>
          {


            //eventContext.FailRequest("not authorized");
          },
          OnBeforeCreateAsync = async eventContext =>
          {
            string filename = eventContext.Metadata["filename"].GetString(System.Text.Encoding.UTF8);
            string filetype = eventContext.Metadata["filetype"].GetString(System.Text.Encoding.UTF8);

            Console.WriteLine(filename);

          }

        }
      };


    }

  }
}
