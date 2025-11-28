using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace MyWebApp.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _env;
        public ImageService(IWebHostEnvironment env) { _env = env; }

        public string SaveProfile(IFormFile file) => SaveFile(file, "images/profiles");
        public string SaveCategoryImage(IFormFile file) => SaveFile(file, "images/categories");
        public string SaveProductImage(IFormFile file) => SaveFile(file, "images/products");

        private string SaveFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return string.Empty;
            var uploads = Path.Combine(_env.WebRootPath, folder);
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return $"/{folder}/{fileName}".Replace("\\", "/");
        }

        public void DeleteFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var path = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(_env.WebRootPath, path);
            if (File.Exists(full)) File.Delete(full);
        }
    }
}
