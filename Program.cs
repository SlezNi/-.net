using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"));

builder.Services.AddScoped<ImageService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.People.Any())
    {
        db.People.Add(new MyWebApp.Models.Person
        {
            FirstName = "Іван",
            LastName = "Скрекотень",
            Bio = "Тіпікал Програміст",
            ProfileImagePath = null
        });
        db.SaveChanges();
    }
}

app.Run();
