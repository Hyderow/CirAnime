using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CirAnime.Data;
using CirAnime.Models;
//using System.Text.Json;
using Newtonsoft.Json;

namespace CirAnime.Pages
{
  public class VideoModel : PageModel
  {
    private readonly CirAnime.Data.CirAnimeContext _context;

    public VideoModel(CirAnime.Data.CirAnimeContext context)
    {

      _context = context;
    }

    public MediaInfo MediaInfo { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
      var v = RouteData.Values;
      var id = int.Parse(v["videoid"].ToString());
      MediaInfo = await _context.MediaInfo.Where(m => m.ID == id).Include("sources").FirstOrDefaultAsync();
      if (MediaInfo == null)
      {
        return new StatusCodeResult(404);
      }

      return new JsonResult(MediaInfo);

    }
  }
}
