using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Data;
using LibraryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        // List - GET
        public IActionResult Details()
        {
            var authors = _context.Authors
                .ToList();

            var authorViewModels = authors.Select(a => new AuthorViewModel
            {
                AuthorId = a.AuthorId,
                AuthorName = a.AuthorName,
            }).ToList();

            return View(authorViewModels);
        }

        // create - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new AuthorViewModel
            {
                AuthorName = "",
            });
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorViewModel authorViewModel)
        {
            if (ModelState.IsValid)
            {
                var author = new Author
                {
                    AuthorName = authorViewModel.AuthorName
                };

                _context.Authors.Add(author);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }
            return View(authorViewModel);
        }

        // Edit - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var author = _context.Authors
                .FirstOrDefault(a => a.AuthorId == id);

            if (author == null) return NotFound();

            var authorViewModel = new AuthorViewModel
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
            };

            return View(authorViewModel);
        }

        // Edit - POST
        [HttpPost]
        public IActionResult Edit(int id, AuthorViewModel authorViewModel)
        {
            if (id != authorViewModel.AuthorId) return BadRequest();

            if (ModelState.IsValid)
            {
                var author = _context.Authors.Find(id);
                if (author == null) return NotFound();

                author.AuthorName = authorViewModel.AuthorName;

                _context.Authors.Update(author);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            return View(authorViewModel);
        }

        // Delete - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var author = _context.Authors
                .FirstOrDefault(a => a.AuthorId == id);

            if (author == null) return NotFound();

            var authorViewModel = new AuthorViewModel
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
            };

            return View(authorViewModel);
        }

        // Delete - POST
        [HttpPost]
        [ValidateAntiForgeryToken] // make sure to validate the request
        public IActionResult DeleteConfirmed(AuthorViewModel authorViewModel)
        {
            var author = _context.Authors
                .Include(a => a.Books)
                .FirstOrDefault(a => a.AuthorId == authorViewModel.AuthorId);

            if (author == null) return NotFound();

            if (author.Books.Any())
            {
                ViewBag.ErrorMessage = "Cannot delete author with existing books.";
                return View("Delete", authorViewModel);
            }

            _context.Authors.Remove(author);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
    }
}