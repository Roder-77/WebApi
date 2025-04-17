using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace MVC.PageModels
{
    public class BasePageModel : PageModel
    {
        protected readonly SEOService _seoService;

        public BasePageModel(IServiceProvider serviceProvider)
        {
            _seoService = serviceProvider.GetRequiredService<SEOService>();
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            ViewData["seo"] = await _seoService.Get(1);
            await base.OnPageHandlerExecutionAsync(context, next);
        }
    }
}
