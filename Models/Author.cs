using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


public class Author
{
    public int AuthorId { get; set; }
    public required string AuthorName { get; set; }
    // add navigation properties
    [JsonIgnore]
    public List<Book> Books { get; set; } = new List<Book>();
}