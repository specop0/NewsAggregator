namespace Parser
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            var database = this.HttpContext.RequestServices.GetService<Database>();
            var parser = this.HttpContext.RequestServices.GetService<Parser>();

            var relevantEntries = database.ParseEntries(parser);
            // var relevantEntries = database.GetEntries();

            return this.Content(relevantEntries.ToHtml(), "text/html;charset=utf-8");
        }
    }
}