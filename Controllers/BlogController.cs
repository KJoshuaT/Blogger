using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChitTalk.Data;
using ChitTalk.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace ChitTalk.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Blog
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blog.ToListAsync());
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blog/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] Blog blog, IFormFile? ImagePath)
        {
            if (ImagePath != null)
            {
                // Generate a unique file name to avoid overwriting
                var fileName = Path.GetFileNameWithoutExtension(ImagePath.FileName);
                var extension = Path.GetExtension(ImagePath.FileName);
                //Use DateTime to create unique files and prevent overwriting similar files.
                var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";

                //Store into server for display
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(stream);
                }

                // Store the file into database
                blog.ImagePath = $"/images/{uniqueFileName}";
            }

            blog.PublishedDate = DateTime.Now;

            blog.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(blog);
        }

        // GET: Blog/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (blog.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToAction("Details", new { id = blog.Id });
            }

            return View(blog);
        }

        // POST: Blog/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,PublishedDate")] Blog blog, IFormFile? ImagePath)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            var existingBlog = await _context.Blog.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (existingBlog == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existingBlog.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToAction("Details", new { id = blog.Id });
            }

            blog.CreatedByUserId = existingBlog.CreatedByUserId;

            // Handle ImagePath logic
            if (ImagePath != null)
            {
                var fileName = Path.GetFileNameWithoutExtension(ImagePath.FileName);
                var extension = Path.GetExtension(ImagePath.FileName);
                var newFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagePath.CopyToAsync(stream);
                }

                blog.ImagePath = $"/images/{newFileName}";
            }
            else
            {
                blog.ImagePath = existingBlog.ImagePath;
            }

            blog.PublishedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(blog).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }



        // GET: Blog/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToAction("Details", new { id = id }); // Change this line
            }

            return View(blog);
        }

        // POST: Blog/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blog.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get current user's ID
            if (blog.CreatedByUserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Access denied.";
                return RedirectToAction("Details", new { id = blog.Id });
            }

            if (!string.IsNullOrEmpty(blog.ImagePath))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", blog.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Searching method
        public async Task<IActionResult> Search(string searchString)
        {
            // Get all blog posts
            var blogs = from b in _context.Blog
                        select b;

            // If search string is not empty
            if (!String.IsNullOrEmpty(searchString))
            {
                // Filter blog posts by title
                blogs = blogs.Where(s => s.Title.Contains(searchString));
            }

            // Return the filtered blog posts
            return View("Index", await blogs.ToListAsync());
        }


        private bool BlogExists(int id)
        {
            return _context.Blog.Any(e => e.Id == id);
        }
    }
}
