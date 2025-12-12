using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class LibraryBranch
{
    public int LibraryBranchId { get; set; }
    public required string BranchName { get; set; }
    // add navigation properties
    [JsonIgnore]
    public List<Book> Books { get; set; } = new List<Book>(); 
    [JsonIgnore]
    public List<Customer> Customers { get; set; } = new List<Customer>(); 
}