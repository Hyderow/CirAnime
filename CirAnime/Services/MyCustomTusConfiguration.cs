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
using System.Text.Json;
using tusdotnet.Extensions;

namespace CirAnime.Services
{
  public class MyCustomTusConfiguration //: DefaultTusConfiguration
  {
    private readonly CirAnimeContext _db;
    private readonly IHttpContextAccessor _httpContext;
    private readonly IConfiguration _config;
    public MyCustomTusConfiguration(CirAnimeContext dbcontext, IHttpContextAccessor ctx, IConfiguration config) : base()
    {
      
      _db = dbcontext;
      _httpContext = ctx;
      _config = config;
      //var jsonmed = "{\"title\": \"Test Video\",\"duration\": 10,\"live\": false,\"thumbnail\": \"https://example.com/thumb.jpg\",\"sources\": [{\"ID\":0, \"url\": \"https://example.com/video.mp4\",\"contentType\": \"video/mp4\",\"quality\": 1080,\"bitrate\": 5000}],\"textTracks\": [{\"ID\":0, \"url\": \"https://example.com/subtitles.vtt\",\"contentType\": \"text/vtt\",\"name\": \"English Subtitles\"}]}";
      //var medinfo = JsonSerializer.Deserialize<MediaInfo>(jsonmed);
      //medinfo.sources.Add()
    }
    private MediaInfo createMediaInfo()
    {
      return new MediaInfo();
    }

    public DefaultTusConfiguration GetTusConfiguration()
    {
      if (!_httpContext.HttpContext.User.Identity.IsAuthenticated)
      {
        return null;
      }

      string uploadDirectory = buildUploadDirectoryPath();
      return new DefaultTusConfiguration
      {
        Store = new TusDiskStore(uploadDirectory),
        UrlPath = "/uploadTus",// + httpContext.User.Identity,
        Events = new tusdotnet.Models.Configuration.Events
        {
          
          OnFileCompleteAsync = async eventContext => {
            var id = UInt64.Parse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ITusFile file = await eventContext.GetFileAsync();

            var metaData = await file.GetMetadataAsync(eventContext.CancellationToken);

            string filename = metaData["filename"].GetString(System.Text.Encoding.UTF8);
            System.IO.File.Move(buildUploadDirectoryPath() + file.Id, buildUploadDirectoryPath() + filename);
            TusDiskStore st = new TusDiskStore(buildUploadDirectoryPath());
            await st.DeleteFileAsync(eventContext.FileId, eventContext.CancellationToken);
            User u = _db.User.Where(stud => stud.DiscordID == id).FirstOrDefault();

            UploadEntry entry = new UploadEntry
            {
              OriginalFileName = filename,
              User = u,
              Title = filename.Contains('.') ? filename.Remove(filename.LastIndexOf('.')) : filename,
              UploadDate = DateTime.Now,

              MediaInfo = new MediaInfo
              {
                title = filename.Contains('.') ? filename.Remove(filename.LastIndexOf('.')) : filename,
                duration = 0,
                thumbnail = "https://cirani.me/circomfy.png",
                live = false
              }
            };

            ProcessingJob p = new ProcessingJob
            {
              OriginalFile = filename,
              UploadEntry = entry,
              Status = ProcessingStatus.Pending,
              Type = ProcessingType.PreProcessing,
              CreationDate = DateTime.UtcNow,
              Progress = 0,
              Quality = 0
            };

            entry.ProcessingJobs.Add(p);
            _db.UploadEntry.Add(entry);
            _db.ProcessingJob.Add(p);
            await _db.SaveChangesAsync();

            Console.WriteLine("file finished");
            // await ProcessFile(file);

          },
          OnAuthorizeAsync = async eventContext =>
          {
            var claims = _httpContext.HttpContext.User.Claims.ToList();
            var id = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id != "142933035740954624")
              eventContext.FailRequest("not authorized");
          },
          OnBeforeCreateAsync = async eventContext => BeforeCreateHandler(eventContext)
        }
      };
    }

    private readonly List<string> validExtensions = new List<string>();
    private bool verifyFileExtension(string fileName)
    {
      var extension = fileName.Split(".").Last<string>();
      return fileName.Contains(extension);
    }

    private async Task BeforeCreateHandler(tusdotnet.Models.Configuration.BeforeCreateContext eventContext)
    {
      if (eventContext.UploadLength > _config.GetValue<long>("MaximumUploadLength"))
      {
        eventContext.FailRequest("Maximum Filesize is: ");
      }
        string filename = eventContext.Metadata["filename"].GetString(System.Text.Encoding.UTF8);
        string filetype = eventContext.Metadata["filetype"].GetString(System.Text.Encoding.UTF8);

        if (!verifyFileExtension(filename))
        {
          eventContext.FailRequest("Unsupported filetype");
        }

        string uploadDirectory = buildUploadDirectoryPath();
        if (!Directory.Exists(uploadDirectory))
        {
          Directory.CreateDirectory(uploadDirectory);
        }
    }


    private string buildUploadDirectoryPath()
    {
      string baseDirectory = _config.GetValue<string>("FileUploadPath");
      if (!baseDirectory.EndsWith("/"))
        baseDirectory.Append('/');
      //string usersubdirectory = _httpContext.HttpContext.User.Identity.Name;
      //usersubdirectory = usersubdirectory.Replace('/', '_').Replace("..", "_");
      //string uploadDirectory = baseDirectory + usersubdirectory + "/";
      return baseDirectory;
     // return uploadDirectory;
      

    }

    
  }
}
