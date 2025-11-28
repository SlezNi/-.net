using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ImageService _img;
        public ProductsController(AppDbContext db, ImageService img) { _db = db; _img = img; }

        public async Task<IActionResult> Index()
        {
            var items = await _db.Products.Include(p => p.Category).Include(p => p.Images).ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.Products.Include(p => p.Category).Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _db.Categories.ToList();
            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, List<IFormFile> images)
        {
            if (!ModelState.IsValid) { ViewBag.Categories = _db.Categories.ToList(); return View(model); }
            _db.Products.Add(model);
            await _db.SaveChangesAsync();

            foreach (var file in images ?? new List<IFormFile>())
            {
                if (file.Length == 0) continue;
                var path = _img.SaveProductImage(file);
                var img = new ProductImage { ProductId = model.Id, ImagePath = path };
                _db.ProductImages.Add(img);
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (model == null) return NotFound();
            ViewBag.Categories = _db.Categories.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product model, List<IFormFile> newImages, int? setMainImageId, int[]? deleteImageIds)
        {
            var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            if (deleteImageIds != null)
            {
                foreach (var imgId in deleteImageIds)
                {
                    var pi = product.Images.FirstOrDefault(x => x.Id == imgId);
                    if (pi != null)
                    {
                        _img.DeleteFile(pi.ImagePath);
                        _db.ProductImages.Remove(pi);
                    }
                }
            }

            foreach (var file in newImages ?? new List<IFormFile>())
            {
                if (file.Length == 0) continue;
                var path = _img.SaveProductImage(file);
                _db.ProductImages.Add(new ProductImage { ProductId = product.Id, ImagePath = path });
            }

            if (setMainImageId.HasValue)
            {
                foreach (var pi in product.Images)
                {
                    pi.IsMain = pi.Id == setMainImageId.Value;
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            foreach (var img in product.Images)
            {
                _img.DeleteFile(img.ImagePath);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var img = await _db.ProductImages.FindAsync(imageId);
            if (img == null) return NotFound();
            _img.DeleteFile(img.ImagePath);
            _db.ProductImages.Remove(img);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
