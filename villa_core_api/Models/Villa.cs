#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.Models;

public class Villa
{
    [Key]
    public int Id {set; get;}

    [Required]
    [MaxLength(255)]
    public string Name {set; get;}

    [Required]
    [Range(1, 1000, ErrorMessage = "Diện tích Villa phải từ 1 đến 100")]
    public int Sqft {set; get;}

    [Required]
    [Range(1, 100, ErrorMessage = "Số người ở Villa phải từ 1 đến 100")]
    public int Occupancy {set; get;}

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

}