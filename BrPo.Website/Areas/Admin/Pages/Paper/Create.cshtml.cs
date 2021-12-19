using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.Paper.Services;

namespace BrPo.Website.Areas.Admin.Pages.Paper
{
    [BindProperties(SupportsGet = true)]
    public class CreateModel : PageModel
    {
        private readonly IPaperService _paperService;

        public CreateModel(
            IPaperService paperService)
        {
            _paperService = paperService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        //public IActionResult OnGet(PaperModel paperModel)
        //{
        //    PaperModel = paperModel;
        //    return Page();
        //}
        public PaperModel PaperModel { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(PaperModel paperModel)
        {
            if (ModelState.IsValid)
            {
                await _paperService.CreatePaperRecord(paperModel);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
