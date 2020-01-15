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
    public class DeleteModel : PageModel
    {
        private readonly CirAnime.Data.CirAnimeContext _context;

        public DeleteModel(CirAnime.Data.CirAnimeContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UploadEntry UploadEntry { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UploadEntry = await _context.UploadEntry
                .Include(u => u.User).FirstOrDefaultAsync(m => m.ID == id);

            if (UploadEntry == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UploadEntry = await _context.UploadEntry.FindAsync(id);

            if (UploadEntry != null)
            {
                _context.UploadEntry.Remove(UploadEntry);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
