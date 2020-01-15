using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CirAnime.Data;
using CirAnime.Models;

namespace CirAnime.Pages.UploadEntries
{
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
            UploadEntry = await _context.UploadEntry
                .Include(u => u.User).ToListAsync();
        }
    }
}
