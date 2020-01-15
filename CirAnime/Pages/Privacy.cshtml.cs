using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CirAnime.Pages
{
  [Authorize(Policy = "Hoster")]
  public class PrivacyModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;
    private readonly ICarService _service;
    public PrivacyModel(ILogger<PrivacyModel> logger, ICarService service)
    {
      _logger = logger;
      _service = service;
    }

    public string Param { get; set; }
    public JsonResult OnGet(string param)
    {
      Param = param;

      return new JsonResult(_service.ReadAll());
    }

  }

  public class ContentManifest
  {
    public string Title { get; set; }
    public int Duration { get; set; }
    public bool Live { get; set; }
    public string Thumbnail { get; set; }
    public List<Source> Sources { get; set; }
    public List<TextTrack> TextTracks { get; set; }
  }

  public interface ICarService
  {
    List<ContentManifest> ReadAll();
  }

  public class CarService : ICarService
  {
    public List<ContentManifest> ReadAll()
    {
      List<Source> sources = new List<Source>
      {
        new Source{Url="https://hydro8182.github.io/robot-animation.mp4", ContentType="video/mp4", Quality=720}
      };
      List<ContentManifest> cars = new List<ContentManifest>{
        
            new ContentManifest{Title="Testtitle", Duration=60,Live=false, Sources=sources},
       //     new ContentManifest{Id = 2, Make="Aston Martin",Model="Rapide",Year=2010,Doors=2,Colour="Black",Price=54995},
       //     new ContentManifest{Id = 3, Make="Porsche",Model=" 911 991",Year=2016,Doors=2,Colour="White",Price=155000},
       //     new ContentManifest{Id = 4, Make="Mercedes-Benz",Model="GLE 63S",Year=2017,Doors=5,Colour="Blue",Price=83995},
       //     new ContentManifest{Id = 5, Make="BMW",Model="X6 M",Year=2016,Doors=5,Colour="Silver",Price=62995},
        };
      return cars;
    }
  }

  public class Source
  {
    public string Url { get; set; }
    public string ContentType { get; set; }
    public int Quality { get; set; }
    public int Bitrate { get; set; }
  }
  public class TextTrack
  {
    public string Url { get; set; }
    public string ContentType { get; set; }
    public string Name { get; set; }
  }
}
