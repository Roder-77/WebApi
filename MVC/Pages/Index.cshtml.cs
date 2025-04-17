using MVC.PageModels;

namespace MVC.Pages
{
    public class IndexModel : BasePageModel
    {
        public IndexModel(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        public void OnGet()
        { }
    }
}
