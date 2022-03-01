using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BrPo.Website.Data;
using BrPo.Website.Services.Paper.Models;

namespace BrPo.Website.Areas.Admin.Pages.Paper;

public class EditModel : PageModel
{
    private readonly BrPo.Website.Data.ApplicationDbContext _context;

    public EditModel(BrPo.Website.Data.ApplicationDbContext context)
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

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(PaperModel).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PaperModelExists(PaperModel.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("/Paper/Index", new { area = "Admin" });
    }

    private bool PaperModelExists(int id)
    {
        return _context.Papers.Any(e => e.Id == id);
    }
}