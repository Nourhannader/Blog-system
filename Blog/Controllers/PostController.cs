using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;


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
        public async Task<IActionResult> Edit(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var postviewodel = new PostViewModel()
            {
                Post = post,
                Categories = new SelectList(context.Categories.ToList(), "Id", "Name", post.CategoryId)
            };
            return View(postviewodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( PostViewModel postViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(postViewModel);
            }

            var postFromDb = await context.Posts.AsNoTracking().FirstOrDefaultAsync(
                p => p.Id == postViewModel.Post.Id);

            if (postFromDb == null)
            {
                return NotFound();
            }

            if (postViewModel.FeatureImage != null)
            {
                var inputFileExtension = Path.GetExtension(postViewModel.FeatureImage.FileName).ToLower();
                bool isAllowed = allowedExtensions.Contains(inputFileExtension);
                if (!isAllowed)
                {
                    ModelState.AddModelError("Image", "Invalid image format. Allowed formats are .jpg, .jpeg, .png");
                    return View(postViewModel);
                }

                var existingFilePath = Path.Combine(webHostEnvironment.WebRootPath, "images",
                    Path.GetFileName(postFromDb.FeatureImageUrl));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
                postViewModel.Post.FeatureImageUrl = await ProcessUploadedFile(postViewModel.FeatureImage);
            }
            else
            {
                postViewModel.Post.FeatureImageUrl = postFromDb.FeatureImageUrl;
            }

            context.Posts.Update(postViewModel.Post);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
           
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(post.FeatureImageUrl))
            {
                var filePath=Path.Combine(webHostEnvironment.WebRootPath,"images",Path.GetFileName(post.FeatureImageUrl));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult cansel()
        {
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var post = context.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
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
