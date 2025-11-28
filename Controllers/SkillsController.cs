using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class SkillsController : Controller
    {
        private readonly AppDbContext _db;
        public SkillsController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(int personId)
        {
            var person = await _db.People.Include(p => p.Skills).FirstOrDefaultAsync(p => p.Id == personId);
            if (person == null) return NotFound();
            return View(person);
        }

        public IActionResult Create(int personId)
        {
            var model = new Skill { PersonId = personId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Skill model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Skills.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new { personId = model.PersonId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var skill = await _db.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            return View(skill);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Skill model)
        {
            if (!ModelState.IsValid) return View(model);
            var skill = await _db.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            skill.Name = model.Name;
            skill.Level = model.Level;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new { personId = skill.PersonId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _db.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            var personId = skill.PersonId;
            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new { personId });
        }
    }
}
