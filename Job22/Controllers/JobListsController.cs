using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Job22.Data;
using Job22.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace Job22.Controllers
{
    [Authorize(Roles = "Employer")] // Đảm bảo tất cả hành động trong controller này đều yêu cầu người dùng có vai trò là Employer
    public class JobListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobListsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobLists
        public async Task<IActionResult> Index()
        {
            // Thêm logic để lọc JobList dựa trên EmployerId nếu cần
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Người dùng không được tìm thấy.");
            }

            // Tìm employerId dựa trên UserId của người dùng hiện tại
            var employer = await _context.Employer.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (employer == null)
            {
                return NotFound("Thông tin ứng viên không được tìm thấy.");
            }

            // Lấy danh sách các Application mà Candidate này đã đăng ký
            var joblists = await _context.JobList
                                        .Include(a => a.Application) // Bao gồm thông tin JobList nếu cần
                                        .Where(a => a.EmployerId == employer.Id)
                                        .ToListAsync();

            return View(joblists);
        }

        // GET: JobLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobList = await _context.JobList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobList == null)
            {
                return NotFound();
            }

            return View(jobList);
        }

        // GET: JobLists/Create
        public IActionResult Create()
        {
            return View();
        }

        

        // POST: JobLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Requirement,Benefit,Location,WorkingTime")] JobList jobList)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // Nếu không tìm thấy thông tin người dùng, trả về lỗi
                    return NotFound("Không tìm thấy thông tin người dùng.");
                }
                var employer = await _context.Employer.FirstOrDefaultAsync(e => e.UserId == user.Id);
                if (employer == null)
                {
                    return NotFound("Không tìm thấy thông tin Employer.");
                }
                jobList.EmployerId = employer.Id; // Gán EmployerId
                _context.JobList.Add(jobList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobList);
        }


        // GET: JobLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobList = await _context.JobList.FindAsync(id);
            if (jobList == null)
            {
                return NotFound();
            }
            return View(jobList);
        }

        // POST: JobLists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Requirement,Benefit,Location,WorkingTime")] JobList jobList)
        {
            if (id != jobList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm và tải thông tin ứng dụng hiện có từ database
                    var existingJobList = await _context.JobList.FindAsync(id);

                    // Kiểm tra xem có tìm thấy ứng dụng không
                    if (existingJobList == null)
                    {
                        return NotFound("Không tìm thấy ứng dụng.");
                    }

                    // Cập nhật trường Image từ dữ liệu được gửi lên
                    existingJobList.Title = jobList.Title;
                    existingJobList.Description = jobList.Description;
                    existingJobList.Requirement = jobList.Requirement;
                    existingJobList.Benefit = jobList.Benefit;
                    existingJobList.Location = jobList.Location;
                    existingJobList.WorkingTime = jobList.WorkingTime;

                    // Cập nhật ứng dụng vào database
                    _context.Update(existingJobList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobListExists(jobList.Id))
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
            return View(jobList);
        }

        // GET: JobLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobList = await _context.JobList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobList == null)
            {
                return NotFound();
            }

            return View(jobList);
        }

        // POST: JobLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobList = await _context.JobList.FindAsync(id);
            if (jobList != null)
            {
                _context.JobList.Remove(jobList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobListExists(int id)
        {
            return _context.JobList.Any(e => e.Id == id);
        }
    }
}
