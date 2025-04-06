using System;
using System.ComponentModel.DataAnnotations;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    public string Location { get; set; }
}
