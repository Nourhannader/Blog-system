using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext context;
        public CategoryController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> create(Category category)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            var postsQuery = context.Posts.Include(p => p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            var posts = postsQuery.ToList();
            ViewData["Categories"] = context.Categories.ToList();
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Update(category);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var categoryFromDb = await context.Categories.FindAsync(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            context.Categories.Remove(categoryFromDb);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
