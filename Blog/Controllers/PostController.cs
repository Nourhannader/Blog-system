using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace Blog.Controllers
{
    public class PostController : Controller
    {

        private readonly AppDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var post = new PostViewModel()
            {
                Categories = new SelectList(context.Categories.ToList(), "Id", "Name"),
            };
            
            return View(post);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                var fileExtension = Path.GetExtension(post.FeatureImage.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("FeatureImage", "Invalid image format. Allowed formats are: " + string.Join(", ", allowedExtensions));
                    post.Categories = new SelectList(context.Categories.ToList(), "Id", "Name");
                    return View(post);
                }
                if (post.FeatureImage != null)
                {
                    post.Post.FeatureImageUrl = await ProcessUploadedFile(post.FeatureImage);
                }
                context.Posts.Add(post.Post);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            post.Categories = new SelectList(context.Categories.ToList(), "Id", "Name");
            return View(post);
        }

        [HttpGet]
        public IActionResult Index(int? categoryId)
        {
            var postsQuery = context.Posts.Include(p =>p.Category).AsQueryable();
            if (categoryId.HasValue)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            var posts = postsQuery.ToList();
            ViewData["Categories"]=context.Categories.ToList();
            return View(posts);
        }
        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var imagesFloderPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(imagesFloderPath))
            {
                Directory.CreateDirectory(imagesFloderPath);
            }
            var filePath = Path.Combine(imagesFloderPath, fileName);
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create)) 
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                //log exception
                return "Error Uploading Image: " + ex.Message;
            }
            return fileName;
        }
    }
}
