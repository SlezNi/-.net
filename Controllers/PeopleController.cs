using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class PeopleController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ImageService _img;
        public PeopleController(AppDbContext db, ImageService img) { _db = db; _img = img; }

        public async Task<IActionResult> Edit(int id)
        {
            var person = await _db.People.FindAsync(id);
            if (person == null) return NotFound();
            return View(person);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Person model, IFormFile? profileImage)
        {
            var person = await _db.People.FindAsync(id);
            if (person == null) return NotFound();

            if (!ModelState.IsValid) return View(model);

            person.FirstName = model.FirstName;
            person.LastName = model.LastName;
            person.Bio = model.Bio;

            if (profileImage != null && profileImage.Length > 0)
            {
                if (!string.IsNullOrEmpty(person.ProfileImagePath)) _img.DeleteFile(person.ProfileImagePath);
                person.ProfileImagePath = _img.SaveProfile(profileImage);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
