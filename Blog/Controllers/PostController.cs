using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Blog.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;


namespace Blog.Controllers
{
    public class PostController : Controller
    {

        private readonly AppDbContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
            this.userManager = userManager;
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
              
                // Get logged-in user ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var postDb = new Post()
                {
                    Title = post.Title,
                    Content = post.Content.Trim(),
                    userId = userId,
                    CategoryId = post.CategoryId
                };
                
                if (post.FeatureImage != null)
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
                        postDb.FeatureImageUrl = await ProcessUploadedFile(post.FeatureImage);
                    }
                }

                context.Posts.Add(postDb);
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
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                
                Categories = new SelectList(context.Categories.ToList(), "Id", "Name", post.CategoryId)
            };
            ViewBag.ExistingImage = post.FeatureImageUrl;

            return View(postviewodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostViewModel postViewModel)
        {
            if (!ModelState.IsValid)
            {
                postViewModel.Categories = new SelectList(context.Categories.ToList(), "Id", "Name", postViewModel.CategoryId);
                return View(postViewModel);
            }

            var postFromDb = await context.Posts.AsNoTracking().FirstOrDefaultAsync(
                p => p.Id == id);

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
                postFromDb.FeatureImageUrl = await ProcessUploadedFile(postViewModel.FeatureImage);
            }
            else
            {
                postFromDb.FeatureImageUrl = postFromDb.FeatureImageUrl;
            }
            postFromDb.Content = postViewModel.Content;
            postFromDb.PublishedDate = DateTime.Now;


            context.Posts.Update(postFromDb);
            await context.SaveChangesAsync();
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
        
        [HttpGet]
        public IActionResult Details(int id)
        {
            var post = context.Posts
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .Include(p=> p.User)
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
            var postsQuery = context.Posts
                            .Include(p => p.Category)
                            .Include(p=>p.Comments)
                            .Include(p=> p.User)
                            .OrderByDescending(p => p.PublishedDate)
                            .AsQueryable();
            if (categoryId.HasValue)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            var posts = postsQuery.ToList();

            

            // Fixing the syntax errors in the assignment to ViewData["Categories"]
            ViewData["Categories"] = context.Categories.ToList();

            return View(posts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Index");
            }
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var comment = new Comment()
            {
                Content = content,
                PostId = postId,
                UserName = user.UserName,
                UserProfileImage=user.ProfilePictureUrl,
                CommentDate = DateTime.Now
            };

            context.Comments.Add(comment);
            context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public IActionResult RemoveComment(int id)
        {
            var comment = context.Comments.FirstOrDefault(c => c.Id == id);

            if (comment == null)
            {
                return RedirectToAction("Index");
            }
            //check permission
            if(User.Identity.Name != comment.UserName && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }

            context.Comments.Remove(comment);
            context.SaveChanges();

            return RedirectToAction("Index");
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
