using JobSearchWebsite.Data;
using Microsoft.AspNetCore.Mvc;

namespace JobSearchWebsite.MVC.Controllers
{
    public class KeywordController : Controller
    {
        private readonly AppDbContext _dbContext;

        public KeywordController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var keywords = _dbContext.Keywords;
            return View(keywords);
        }
    }
}
