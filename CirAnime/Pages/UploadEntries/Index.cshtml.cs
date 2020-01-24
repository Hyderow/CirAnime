using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CirAnime.Data;
using CirAnime.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CirAnime.Pages.UploadEntries
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CirAnime.Data.CirAnimeContext _context;

        public IndexModel(CirAnime.Data.CirAnimeContext context)
        {
            _context = context;
        }

        public IList<UploadEntry> UploadEntry { get;set; }

        public async Task OnGetAsync()
        {
            try
            {
                var uid = UInt64.Parse(Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                UploadEntry = await _context.UploadEntry
                    .Include(u => u.User).Include(u => u.MediaInfo).Include(u => u.MediaInfo.sources).Include(u => u.ProcessingJobs).Where(u => u.User.DiscordID == uid).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            }

        public async Task OnUpdateProgessAsync()
        { 
            

        }
    }
}
