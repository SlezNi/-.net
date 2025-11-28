using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;

namespace MyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            var person = await _db.People.Include(p => p.Skills).FirstOrDefaultAsync();
            return View(person);
        }
    }
}
