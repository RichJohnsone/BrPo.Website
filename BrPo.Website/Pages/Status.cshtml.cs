using BrPo.Website.Services.Email;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrPo.Website.Pages;

[BindProperties(SupportsGet = true)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class StatusModel : PageModel
{
    public string Status { get; set; }

    public int OriginalStatusCode { get; set; }

    public string? OriginalPathAndQuery { get; set; }

    public IActionResult OnGet(string status = null)
    {
        Status = status;
        OriginalStatusCode = status.ToInt();

        if (Status == "404")
        {
            var statusCodeReExecuteFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeReExecuteFeature is not null)
            {
                OriginalPathAndQuery = string.Join(
                    statusCodeReExecuteFeature.OriginalPathBase,
                    statusCodeReExecuteFeature.OriginalPath,
                    statusCodeReExecuteFeature.OriginalQueryString);
            }
            RedirectToPage();
        }

        return Page();
    }
}