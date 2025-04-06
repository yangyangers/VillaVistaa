using System;
using System.ComponentModel.DataAnnotations;

public class Announcement
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DatePosted { get; set; }

}
