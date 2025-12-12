using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // List
        public IActionResult Details()
        {
            var customers = _context.Customers
                .Include(c => c.LibraryBranch)
                .Include(c => c.BorrowedBooks)
                .ToList();

            var customerViewModels = customers.Select(c => new CustomerViewModel
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                BranchName = c.LibraryBranch != null ? c.LibraryBranch.BranchName : "",
                BorrowedBookIds = c.BorrowedBooks.Select(b => b.BookId).ToList(),
                BorrowedBookTitles = c.BorrowedBooks.Select(b => b.BookTitle).ToList(),
                CreatedAt = c.CreatedAt
            }).ToList();

            return View(customerViewModels);
        }

        // Create - GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.LibraryBranches = _context.LibraryBranches.Select(lb => new { lb.LibraryBranchId, lb.BranchName }).ToList();
            ViewBag.Books = _context.Books.Select(b => new { b.BookId, b.BookTitle }).ToList();
            return View(new CustomerViewModel
            {
                CustomerName = "",
                BranchName = "",
                BorrowedBookIds = new List<int>(),
                BorrowedBookTitles = null,
                CreatedAt = DateTime.Now
            });
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                var libraryBranch = _context.LibraryBranches
                    .FirstOrDefault(lb => lb.BranchName == customerViewModel.BranchName);

                if (libraryBranch == null)
                {
                    ModelState.AddModelError("BranchName", "Invalid library branch.");
                    ViewBag.LibraryBranches = _context.LibraryBranches.Select(lb => new { lb.LibraryBranchId, lb.BranchName }).ToList();
                    ViewBag.Books = _context.Books.Select(b => new { b.BookId, b.BookTitle }).ToList();
                    return View(customerViewModel);
                }

                var customer = new Customer
                {
                    CustomerName = customerViewModel.CustomerName,
                    CreatedAt = DateTime.Now,
                    LibraryBranchId = libraryBranch.LibraryBranchId,
                    BorrowedBooks = new List<Book>()
                };

                if (customerViewModel.BorrowedBookIds != null && customerViewModel.BorrowedBookIds.Any())
                {
                    customerViewModel.BorrowedBookIds.RemoveAll(id => id == -1);
                    var books = _context.Books
                        .Where(b => customerViewModel.BorrowedBookIds.Contains(b.BookId))
                        .ToList();
                    customer.BorrowedBooks.AddRange(books);
                }
                
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            ViewBag.LibraryBranches = _context.LibraryBranches.Select(lb => new { lb.LibraryBranchId, lb.BranchName }).ToList();
            ViewBag.Books = _context.Books.Select(b => new { b.BookId, b.BookTitle }).ToList();
            return View(customerViewModel);
        }

        // Edit - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers
                .Include(c => c.LibraryBranch)
                .Include(c => c.BorrowedBooks)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null) return NotFound();

            var customerViewModel = new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                BranchName = customer.LibraryBranch != null ? customer.LibraryBranch.BranchName : "",
                BorrowedBookIds = customer.BorrowedBooks.Select(b => b.BookId).ToList(),
                BorrowedBookTitles = customer.BorrowedBooks.Select(b => b.BookTitle).ToList(),
                CreatedAt = customer.CreatedAt
            };

            ViewBag.LibraryBranches = _context.LibraryBranches.Select(lb => new { lb.LibraryBranchId, lb.BranchName }).ToList();
            ViewBag.Books = _context.Books.Select(b => new { b.BookId, b.BookTitle }).ToList();
            return View(customerViewModel);
        }

        // Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CustomerViewModel customerViewModel)
        {
            if (id != customerViewModel.CustomerId) return BadRequest();
            if (ModelState.IsValid)
            {
                var customer = _context.Customers
                    .Include(c => c.BorrowedBooks)
                    .FirstOrDefault(c => c.CustomerId == id);
                if (customer == null) return NotFound();

                var libraryBranch = _context.LibraryBranches
                    .FirstOrDefault(lb => lb.BranchName == customerViewModel.BranchName);

                if (libraryBranch == null)
                {
                    ModelState.AddModelError("BranchName", "Invalid library branch.");
                    ViewBag.LibraryBranches = _context.LibraryBranches.Select(lb => new { lb.LibraryBranchId, lb.BranchName }).ToList();
                    ViewBag.Books = _context.Books.Select(b => new { b.BookId, b.BookTitle }).ToList();
                    return View(customerViewModel);
                }

                customer.CustomerName = customerViewModel.CustomerName;
                customer.LibraryBranchId = libraryBranch.LibraryBranchId;

                customer.BorrowedBooks.Clear();
                if (customerViewModel.BorrowedBookIds != null && customerViewModel.BorrowedBookIds.Any())
                {
                    customerViewModel.BorrowedBookIds.RemoveAll(id => id == -1);
                    var books = _context.Books
                        .Where(b => customerViewModel.BorrowedBookIds.Contains(b.BookId))
                        .ToList();
                    customer.BorrowedBooks.AddRange(books);
                }

                _context.Customers.Update(customer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Details));
            }

            ViewBag.LibraryBranches = _context.LibraryBranches.Select(lb => new { lb.LibraryBranchId, lb.BranchName }).ToList();
            ViewBag.Books = _context.Books.Select(b => new { b.BookId, b.BookTitle }).ToList();
            return View(customerViewModel);
        }

        // Delete - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers
                .Include(c => c.LibraryBranch)
                .Include(c => c.BorrowedBooks)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null) return NotFound();

            var customerViewModel = new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                BranchName = customer.LibraryBranch != null ? customer.LibraryBranch.BranchName : "",
                BorrowedBookIds = customer.BorrowedBooks.Select(b => b.BookId).ToList(),
                BorrowedBookTitles = customer.BorrowedBooks.Select(b => b.BookTitle).ToList(),
                CreatedAt = customer.CreatedAt
            };

            return View(customerViewModel);
        }

        // Delete - POST
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers
                .Include(c => c.BorrowedBooks)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null) return NotFound();

            if (customer.BorrowedBooks.Any())
            {
                ViewBag.ErrorMessage = "Cannot delete customer with borrowed books.";
                var customerViewModel = new CustomerViewModel
                {
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerName,
                    BranchName = customer.LibraryBranch != null ? customer.LibraryBranch.BranchName : "",
                    BorrowedBookIds = customer.BorrowedBooks.Select(b => b.BookId).ToList(),
                    BorrowedBookTitles = customer.BorrowedBooks.Select(b => b.BookTitle).ToList(),
                    CreatedAt = customer.CreatedAt
                };
                return View("Delete", customerViewModel);
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return RedirectToAction(nameof(Details));
        }
    }
}