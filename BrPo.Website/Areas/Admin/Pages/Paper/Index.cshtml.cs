using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BrPo.Website.Services.Paper.Models;
using BrPo.Website.Services.Paper.Services;

namespace BrPo.Website.Areas.Admin.Pages.Paper
{
    public class IndexModel : PageModel
    {
        private readonly IPaperService _paperService;

        public IndexModel(IPaperService paperService)
        {
            _paperService = paperService;
        }

        public IList<PaperModel> PaperModels { get;set; }

        public void OnGetAsync()
        {
            PaperModels = _paperService.GetPapers();
        }
    }
}
