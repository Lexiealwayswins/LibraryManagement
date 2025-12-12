using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class LibraryBranchController : Controller
    {
        private readonly AppDbContext _context;

        public LibraryBranchController(AppDbContext context)
        {
            _context = context;
        }

        // List
        public IActionResult Details()
        {
            var branches = _context.LibraryBranches.ToList();

            var branchViewModels = branches.Select(b => new LibraryBranchViewModel
            {
                LibraryBranchId = b.LibraryBranchId,
                BranchName = b.BranchName
            }).ToList();

            return View(branchViewModels);
        }

        // Create - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new LibraryBranchViewModel
            {
                BranchName = ""
            });
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LibraryBranchViewModel branchViewModel)
        {
            if (ModelState.IsValid)
            {
                var branch = new LibraryBranch
                {
                    BranchName = branchViewModel.BranchName
                };

                _context.LibraryBranches.Add(branch);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            return View(branchViewModel);
        }

        // Edit - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var branch = _context.LibraryBranches
                .FirstOrDefault(b => b.LibraryBranchId == id);

            if (branch == null) return NotFound();

            var branchViewModel = new LibraryBranchViewModel
            {
                LibraryBranchId = branch.LibraryBranchId,
                BranchName = branch.BranchName
            };

            return View(branchViewModel);
        }

        // Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LibraryBranchViewModel branchViewModel)
        {
            if (id != branchViewModel.LibraryBranchId) return BadRequest();

            if (ModelState.IsValid)
            {
                var branch = _context.LibraryBranches.Find(id);
                if (branch == null) return NotFound();

                branch.BranchName = branchViewModel.BranchName;

                _context.LibraryBranches.Update(branch);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            return View(branchViewModel);
        }

        // Delete - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var branch = _context.LibraryBranches
                .FirstOrDefault(b => b.LibraryBranchId == id);

            if (branch == null) return NotFound();

            var branchViewModel = new LibraryBranchViewModel
            {
                LibraryBranchId = branch.LibraryBranchId,
                BranchName = branch.BranchName
            };

            return View(branchViewModel);
        }

        // Delete - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(LibraryBranchViewModel branchViewModel)
        {
            var branch = _context.LibraryBranches
                .Include(b => b.Books)
                .Include(b => b.Customers)
                .FirstOrDefault(b => b.LibraryBranchId == branchViewModel.LibraryBranchId);

            if (branch == null) return NotFound();

            if (branch.Books.Any() || branch.Customers.Any())
            {
                ViewBag.ErrorMessage = "Cannot delete branch with existing books or customers.";
                return View("Delete", branchViewModel);
            }

            _context.LibraryBranches.Remove(branch);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
    }
}