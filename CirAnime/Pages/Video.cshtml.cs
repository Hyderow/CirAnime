using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CirAnime.Data;
using CirAnime.Models;

namespace CirAnime.Pages
{
    public class VideoModel : PageModel
    {
        private readonly CirAnime.Data.CirAnimeContext _context;

        public VideoModel(CirAnime.Data.CirAnimeContext context)
        {
            
            _context = context;
        }

        public MediaInfo MediaInfo { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
      var v = RouteData.Values;
          MediaInfo =  _context.MediaInfo.Find(v["videoid"]);
          return new JsonResult("");
          
        }
    }
}
