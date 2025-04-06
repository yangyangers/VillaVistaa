using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VillaVista.Models;

public class Homeowner
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    [Required]
    public string HouseNumber { get; set; }

    public string Street { get; set; }
}
