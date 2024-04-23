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
using static System.Net.Mime.MediaTypeNames;

namespace Job22.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employers
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Người dùng không được tìm thấy.");
            }

            // Tìm employerid dựa trên UserId của người dùng hiện tại
            var employer = await _context.Employer.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (employer == null)
            {
                return NotFound("Thông tin ứng viên không được tìm thấy.");
            }
            return View(employer);
        }

        // GET: Employers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // GET: Employers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,Name,Position,Company,Email,Phone,Password")] Employer employer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employer);
        }

        // GET: Employers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer.FindAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            return View(employer);
        }

        // POST: Employers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,Name,Position,Company,Email,Phone,Password")] Employer employer)
        {
            if (id != employer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm và tải thông tin ứng dụng hiện có từ database
                    var existingEmployer = await _context.Employer.FindAsync(id);

                    // Kiểm tra xem có tìm thấy ứng dụng không
                    if (existingEmployer == null)
                    {
                        return NotFound("Không tìm thấy ứng dụng.");
                    }

                    // Cập nhật trường Image từ dữ liệu được gửi lên
                    existingEmployer.Image = employer.Image;
                    existingEmployer.Name = employer.Name;
                    existingEmployer.Position = employer.Position;
                    existingEmployer.Company = employer.Company;
                    existingEmployer.Email = employer.Email;
                    existingEmployer.Phone = employer.Phone;
                    existingEmployer.Password = employer.Password;

                    // Cập nhật ứng dụng vào database
                    _context.Update(existingEmployer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployerExists(employer.Id))
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
            return View(employer);
        }

        // GET: Employers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employer = await _context.Employer.FindAsync(id);
            if (employer != null)
            {
                _context.Employer.Remove(employer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployerExists(int id)
        {
            return _context.Employer.Any(e => e.Id == id);
        }
    }
}
