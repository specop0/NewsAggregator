namespace Parser
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            var database = this.HttpContext.RequestServices.GetRequiredService<Database>();
            var browser = this.HttpContext.RequestServices.GetRequiredService<Browser>();

            var relevantEntries = database.ParseEntries(browser);

            return this.Content(relevantEntries.ToHtml(), "text/html;charset=utf-8");
        }
    }
}