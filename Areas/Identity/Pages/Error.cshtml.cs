using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Diagnostics;


namespace LibraryManagement.Areas.Identity.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? ExceptionMessage { get; set; }

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        // public void OnGet()
        // {
        //     RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        //     var exceptionHandlerPathFeature =
        //     HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        //     if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
        //     {
        //         ExceptionMessage = "The file was not found.";
        //     }
        //     if (exceptionHandlerPathFeature?.Path == "/")
        //     {
        //         ExceptionMessage ??= string.Empty;
        //         ExceptionMessage += " Page: Home.";
        //     }
        // }

        public int OriginalStatusCode { get; set; }
        public string? OriginalPathAndQuery { get; set; }
        public void OnGet(int statusCode)
        {
            OriginalStatusCode = statusCode;
            var statusCodeReExecuteFeature =
                HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusCodeReExecuteFeature is not null)
            {
                OriginalPathAndQuery = string.Join(
                statusCodeReExecuteFeature.OriginalPathBase,
                statusCodeReExecuteFeature.OriginalPath,
                statusCodeReExecuteFeature.OriginalQueryString);
            }
        }
    }
}
