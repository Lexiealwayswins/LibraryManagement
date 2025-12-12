using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


public class Book
{
    [Key]
    public int BookId { get; set; }
    public required string BookTitle { get; set; }
    public int AuthorId { get; set; }
    public int LibraryBranchId { get; set; }

    // add navigation properties
    [JsonIgnore]
    public Author? Author { get; set; }
    [JsonIgnore]
    public LibraryBranch? LibraryBranch { get; set; }
    [JsonIgnore]
    public List<Customer> BorrowedByCustomers { get; set; } = new List<Customer>();
}
