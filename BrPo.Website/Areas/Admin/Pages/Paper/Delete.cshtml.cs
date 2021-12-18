using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BrPo.Website.Data;
using BrPo.Website.Services.Paper.Models;

namespace BrPo.Website.Areas.Admin.Pages.Paper
{
    public class DeleteModel : PageModel
    {
        private readonly BrPo.Website.Data.ApplicationDbContext _context;

        public DeleteModel(BrPo.Website.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PaperModel PaperModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PaperModel = await _context.Papers.FirstOrDefaultAsync(m => m.Id == id);

            if (PaperModel == null)
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

            PaperModel = await _context.Papers.FindAsync(id);

            if (PaperModel != null)
            {
                _context.Papers.Remove(PaperModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
