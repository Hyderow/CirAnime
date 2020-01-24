using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CirAnime.Data;
using CirAnime.Models;

namespace CirAnime.Pages
{
    public class uploadedModel : PageModel
    {
        private readonly CirAnime.Data.CirAnimeContext _context;

        public uploadedModel(CirAnime.Data.CirAnimeContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
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
