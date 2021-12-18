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
    public class IndexModel : PageModel
    {
        private readonly BrPo.Website.Data.ApplicationDbContext _context;

        public IndexModel(BrPo.Website.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PaperModel> PaperModel { get;set; }

        public async Task OnGetAsync()
        {
            PaperModel = await _context.Papers.ToListAsync();
        }
    }
}
