using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BrPo.Website.Data;
using BrPo.Website.Services.Paper.Models;

namespace BrPo.Website.Areas.Admin.Pages.Paper
{
    public class CreateModel : PageModel
    {
        private readonly BrPo.Website.Data.ApplicationDbContext _context;

        public CreateModel(BrPo.Website.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PaperModel PaperModel { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Papers.Add(PaperModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
