using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Job22.Data;
using Job22.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Job22.Controllers
{

    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public CandidatesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Candidates
        [Authorize(Roles = "Candidate")] // Đảm bảo tất cả hành động trong controller này đều yêu cầu người dùng có vai trò là Employer

        public async Task<IActionResult> Index()
        {
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
            return View(candidate);
        }

        // GET: Candidates/Details/5
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        // GET: Candidates/Create
        [Authorize(Roles = "Candidate")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Phone,Password")] Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(candidate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(candidate);
        }

        // GET: Candidates/Edit/5
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidate.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }
            return View(candidate);
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Phone,Password")] Candidate candidate)
        {
            if (id != candidate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm và tải thông tin ứng dụng hiện có từ database
                    var existingCandidate = await _context.Candidate.FindAsync(id);

                    // Kiểm tra xem có tìm thấy ứng dụng không
                    if (existingCandidate == null)
                    {
                        return NotFound("Không tìm thấy ứng dụng.");
                    }

                    // Cập nhật trường Image từ dữ liệu được gửi lên
                    existingCandidate.Name = candidate.Name;
                    existingCandidate.Email = candidate.Email;
                    existingCandidate.Phone = candidate.Phone;
                    existingCandidate.Password = candidate.Password;

                    // Cập nhật ứng dụng vào database
                    _context.Update(existingCandidate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandidateExists(candidate.Id))
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
            return View(candidate);
        }


        // GET: Candidates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidate
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candidate = await _context.Candidate.FindAsync(id);
            if (candidate != null)
            {
                _context.Candidate.Remove(candidate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidateExists(int id)
        {
            return _context.Candidate.Any(e => e.Id == id);
        }
    }
}
