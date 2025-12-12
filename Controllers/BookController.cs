using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Data;
using LibraryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        // List all books
        public IActionResult Details()
        {
            // first, load all books with their related authors and library branches
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.LibraryBranch)
                .ToList(); 

            // then, map them to BookViewModel
            var bookViewModels = books.Select(b => new BookViewModel
            {
                BookId = b.BookId,
                Title = b.BookTitle,
                AuthorName = b.Author != null ? b.Author.AuthorName : "", // client side null check
                BranchName = b.LibraryBranch != null ? b.LibraryBranch.BranchName : "" // client side null check
            }).ToList();

            return View(bookViewModels);
        }

        // GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.LibraryBranches = _context.LibraryBranches.ToList();
            return View(new BookViewModel
            {
                Title = "",
                AuthorName = "",
                BranchName = ""
            });
        }

        // Create - POST
        [HttpPost]
        public IActionResult Create(BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    BookTitle = bookViewModel.Title,
                    AuthorId = _context.Authors
                        .FirstOrDefault(a => a.AuthorName == bookViewModel.AuthorName)?.AuthorId ?? 0,
                    LibraryBranchId = _context.LibraryBranches
                        .FirstOrDefault(lb => lb.BranchName == bookViewModel.BranchName)?.LibraryBranchId ?? 0
                };

                if (book.AuthorId == 0 || book.LibraryBranchId == 0)
                {
                    ModelState.AddModelError("", "invalid author or library branch.");
                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.LibraryBranches = _context.LibraryBranches.ToList();
                    return View(bookViewModel);
                }

                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.LibraryBranches = _context.LibraryBranches.ToList();
            return View(bookViewModel);
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.LibraryBranch)
                .FirstOrDefault(b => b.BookId == id);

            if (book == null) return NotFound();

            var bookViewModel = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.BookTitle,
                AuthorName = book.Author != null ? book.Author.AuthorName : "", 
                BranchName = book.LibraryBranch != null ? book.LibraryBranch.BranchName : ""
            };

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.LibraryBranches = _context.LibraryBranches.ToList();
            return View(bookViewModel);
        }

        // Edit - POST
        [HttpPost]
        public IActionResult Edit(int id, BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = _context.Books.Find(id);
                if (book == null) return NotFound();

                book.BookTitle = bookViewModel.Title;
                book.AuthorId = _context.Authors
                    .FirstOrDefault(a => a.AuthorName == bookViewModel.AuthorName)?.AuthorId ?? 0;
                book.LibraryBranchId = _context.LibraryBranches
                    .FirstOrDefault(lb => lb.BranchName == bookViewModel.BranchName)?.LibraryBranchId ?? 0;

                if (book.AuthorId == 0 || book.LibraryBranchId == 0)
                {
                    ModelState.AddModelError("", "invalid author or library branch.");
                    ViewBag.Authors = _context.Authors.ToList();
                    ViewBag.LibraryBranches = _context.LibraryBranches.ToList();
                    return View(bookViewModel);
                }

                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.LibraryBranches = _context.LibraryBranches.ToList();
            return View(bookViewModel);
        }

        // Delete - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.LibraryBranch)
                .FirstOrDefault(b => b.BookId == id);
            if (book == null) return NotFound();

            var bookViewModel = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.BookTitle,
                AuthorName = book.Author != null ? book.Author.AuthorName : "", 
                BranchName = book.LibraryBranch != null ? book.LibraryBranch.BranchName : ""
            };

            return View(bookViewModel);
        }

        // delete - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(BookViewModel bookViewModel)
        {
            var book = _context.Books.Find(bookViewModel.BookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Details));
        }
    }
}