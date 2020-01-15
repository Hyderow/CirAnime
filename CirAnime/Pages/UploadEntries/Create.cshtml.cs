using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CirAnime.Data;
using CirAnime.Models;

namespace CirAnime.Pages.UploadEntries
{
    public class CreateModel : PageModel
    {
        private readonly CirAnime.Data.CirAnimeContext _context;

        public CreateModel(CirAnime.Data.CirAnimeContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["UserID"] = new SelectList(_context.User, "ID", "ID");
            return Page();
        }

        [BindProperty]
        public UploadEntry UploadEntry { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.UploadEntry.Add(UploadEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
