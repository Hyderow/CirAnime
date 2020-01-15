using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CirAnime.Data;
using CirAnime.Models;

namespace CirAnime.Pages.UploadEntries
{
    public class EditModel : PageModel
    {
        private readonly CirAnime.Data.CirAnimeContext _context;

        public EditModel(CirAnime.Data.CirAnimeContext context)
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
           ViewData["UserID"] = new SelectList(_context.User, "ID", "ID");
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(UploadEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UploadEntryExists(UploadEntry.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UploadEntryExists(int id)
        {
            return _context.UploadEntry.Any(e => e.ID == id);
        }
    }
}
