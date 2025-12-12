using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibraryManagement.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public required string CustomerName { get; set; } = string.Empty;

        public required string BranchName { get; set; } = string.Empty;
        
        [Display(Name = "Borrowed Books")]
        public List<int> BorrowedBookIds { get; set; } = new List<int>(); // optional, allows empty list    

        public List<string>? BorrowedBookTitles { get; set; } = null; // optional, allows null 

        public string BorrowedBooksDisplay => BorrowedBookTitles != null ? string.Join(", ", BorrowedBookTitles) : "None";

        public DateTime CreatedAt { get; set; }
    }
}