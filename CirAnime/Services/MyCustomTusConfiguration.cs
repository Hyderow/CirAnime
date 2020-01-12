using CirAnime.Data;
using CirAnime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Stores;
namespace CirAnime.Services
{
  public class MyCustomTusConfiguration : DefaultTusConfiguration
  {
    public MyCustomTusConfiguration(CirAnimeContext dbcontext, IHttpContextAccessor ctx, IConfiguration config) : base()
    {

      if (!ctx.HttpContext.User.Identity.IsAuthenticated)
      {
        return;
      }

      string baseDirectory = config.GetValue<string>("FileUploadPath");
      if (!baseDirectory.EndsWith("/")) baseDirectory.Append('/');


      string usersubdirectory = ctx.HttpContext.User.Identity.Name;
      usersubdirectory = usersubdirectory.Replace('/', '_').Replace("..", "_");


      string uploadDirectory = baseDirectory + usersubdirectory + "/";
      if (!Directory.Exists(uploadDirectory))
      {
        Directory.CreateDirectory(uploadDirectory);
      }
      Store = new TusDiskStore(uploadDirectory);
      UrlPath = "/uploadTus";// + httpContext.User.Identity,

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
           Owner = ctx.HttpContext.User.Identity.Name,
           Title = filename.Contains('.') ? filename.Remove(filename.LastIndexOf('.')) : filename,
           UploadDate = DateTime.Now
         };
         dbcontext.UploadEntry.Add(entry);
         await dbcontext.SaveChangesAsync();

         Console.WriteLine("file finished");
         // await ProcessFile(file);

       },
        OnAuthorizeAsync = async eventContext =>
        {
          Console.WriteLine("");
          var claims = ctx.HttpContext.User.Claims.ToList();
          var id = ctx.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

          var cl = claims[0];
          //eventContext.FailRequest("not authorized");
        },
        OnBeforeCreateAsync = async eventContext =>
        {
          string filename = eventContext.Metadata["filename"].GetString(System.Text.Encoding.UTF8);
          string filetype = eventContext.Metadata["filetype"].GetString(System.Text.Encoding.UTF8);

          Console.WriteLine(filename);

        }

      };
    }

  }
}
