namespace LibraryManagement.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }
        public required string Title { get; set; }
        public required string AuthorName { get; set; }
        public required string BranchName { get; set; }
    }
}