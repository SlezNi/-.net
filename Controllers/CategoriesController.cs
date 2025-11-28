using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ImageService _img;
        public CategoriesController(AppDbContext db, ImageService img) { _db = db; _img = img; }

        public IActionResult Index() => View(_db.Categories.ToList());

        public IActionResult Create() => View(new Category());

        [HttpPost]
        public async Task<IActionResult> Create(Category model, IFormFile? image)
        {
            if (!ModelState.IsValid) return View(model);
            if (image != null) model.ImagePath = _img.SaveCategoryImage(image);
            _db.Categories.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category model, IFormFile? image, bool deleteImage = false)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            cat.Name = model.Name;

            if (deleteImage && !string.IsNullOrEmpty(cat.ImagePath))
            {
                _img.DeleteFile(cat.ImagePath);
                cat.ImagePath = null;
            }

            if (image != null)
            {
                if (!string.IsNullOrEmpty(cat.ImagePath)) _img.DeleteFile(cat.ImagePath);
                cat.ImagePath = _img.SaveCategoryImage(image);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            if (!string.IsNullOrEmpty(cat.ImagePath)) _img.DeleteFile(cat.ImagePath);
            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
