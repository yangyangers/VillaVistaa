using System;
using System.ComponentModel.DataAnnotations;

public class Billing
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string HomeownerId { get; set; } // Foreign key to ApplicationUser

    [Required]
    public decimal AmountDue { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public bool IsPaid { get; set; } = false;

    public DateTime? PaidAt { get; set; }
}
