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
    public class DetailsModel : PageModel
    {
        private readonly BrPo.Website.Data.ApplicationDbContext _context;

        public DetailsModel(BrPo.Website.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
