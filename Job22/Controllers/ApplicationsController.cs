using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job22.Data;
using Job22.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Job22.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ApplicationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Applications
        [Authorize(Roles = "Candidate")]

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Người dùng không được tìm thấy.");
            }

            // Tìm CandidateId dựa trên UserId của người dùng hiện tại
            var candidate = await _context.Candidate.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (candidate == null)
            {
                return NotFound("Thông tin ứng viên không được tìm thấy.");
            }

            // Lấy danh sách các Application mà Candidate này đã đăng ký
            var applications = await _context.Application
                                        .Include(a => a.JobList) // Bao gồm thông tin JobList nếu cần
                                        .Where(a => a.CandidateId == candidate.Id)
                                        .ToListAsync();

            return View(applications);
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Application
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpGet]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var jobListExists = await _context.JobList.FindAsync(id);
            if (jobListExists == null)
            {
                return NotFound("Job list not found.");
            }

            //var applicationModel = new Application { JobListId = jobListId };
            var applicationModel = new Application { JobListId = id.Value }; // Sử dụng id.Value vì id là một Nullable int
            return View(applicationModel); // Truyền applicationModel thay vì jobListExists
        }

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int jobListId, [Bind("Image")] Application application, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(application);
            }

            var user = await _userManager.GetUserAsync(User);
            var candidate = await _context.Candidate.FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (candidate == null)
            {
                return NotFound("Candidate not found.");
            }

            var jobListExists = await _context.JobList.AnyAsync(jl => jl.Id == jobListId);
            if (!jobListExists)
            {
                return NotFound("Job list not found.");
            }

            // Xử lý và lưu ảnh
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Check if the directory exists
                if (!Directory.Exists(imageDirectory))
                {
                    // If it doesn't exist, create the directory
                    Directory.CreateDirectory(imageDirectory);
                }

                var filePath = Path.Combine(imageDirectory, imageFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                application.Image = "/images/" + imageFile.FileName; // Lưu đường dẫn tới ảnh
            }

            application.CandidateId = candidate.Id;
            application.JobListId = jobListId;

            _context.Application.Add(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Application.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image")] Application application, IFormFile imageFile)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm và tải thông tin ứng dụng hiện có từ database
                    var existingApplication = await _context.Application.FindAsync(id);

                    // Kiểm tra xem có tìm thấy ứng dụng không
                    if (existingApplication == null)
                    {
                        return NotFound("Không tìm thấy ứng dụng.");
                    }

                    // Xử lý và lưu file ảnh nếu có
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        var filePath = Path.Combine(directoryPath, imageFile.FileName);

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        existingApplication.Image = "/images/" + imageFile.FileName; // Cập nhật đường dẫn ảnh mới
                    }

                    // Cập nhật ứng dụng vào database
                    _context.Update(existingApplication);
                    // Đánh dấu chỉ cập nhật trường Image
                    _context.Entry(existingApplication).Property("Image").IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
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
            return View(application);
        }

        // GET: Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Application
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var application = await _context.Application.FindAsync(id);
            if (application != null)
            {
                _context.Application.Remove(application);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(int id)
        {
            return _context.Application.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Candidate")]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> JobForCandidates(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var jobListQuery = _context.JobList.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                jobListQuery = jobListQuery.Where(j => j.Title.Contains(searchString));
            }

            var jobList = await jobListQuery.ToListAsync();
            return View(jobList);
        }
        public async Task<IActionResult> ViewApplications(int jobListId)
        {
            var applications = await _context.Application
                                    .Where(a => a.JobListId == jobListId)
                                    .Include(a => a.Candidate) // Bao gồm thông tin Candidate
                                    .ToListAsync();

            // Bạn có thể cần một ViewModel để bao gồm cả thông tin JobList nếu cần
            // Ví dụ: var viewModel = new JobListApplicationsViewModel { Applications = applications, JobList = ... };

            return View(applications); // hoặc return View(viewModel);
        }

    }
}
