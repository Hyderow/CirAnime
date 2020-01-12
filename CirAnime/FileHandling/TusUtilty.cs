using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Stores;
using CirAnime.Data;
using CirAnime.Models;

namespace CirAnime.FileHandling
{
  public class TusUtilty
  {
    private readonly IConfiguration _config;
    private CirAnimeContext _dbContext;
    public TusUtilty(IConfiguration config, CirAnimeContext dbContext)
    {
      _config = config;
      _dbContext = dbContext;
    }
    public DefaultTusConfiguration CreateTusConfiguration(HttpContext httpContext)
    {
      
      if (!httpContext.User.Identity.IsAuthenticated)
      {
        return null;
      }

      string baseDirectory = _config.GetValue<string>("FileUploadPath");
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
            _dbContext.UploadEntry.Add(entry);

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

    public static async Task ProcessFile(ITusFile file)
    {

    }
  }
}
