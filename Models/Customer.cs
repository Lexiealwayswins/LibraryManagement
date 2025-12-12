using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


public class Customer
{
    public int CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int LibraryBranchId { get; set; }
    // add navigation properties
    [JsonIgnore]
    public LibraryBranch? LibraryBranch { get; set; } 
    [JsonIgnore]
    public List<Book> BorrowedBooks { get; set; } = new List<Book>(); 
}