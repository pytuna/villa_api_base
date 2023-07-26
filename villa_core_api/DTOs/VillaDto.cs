#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.DTOs;

public class VillaDto
{
    [Key]
    public int Id {set; get;}

    public string Description {set; get;}

    [Required]
    [MaxLength(255)]
    public string Name {set; get;}

    [Required]
    [Range(1, 1000, ErrorMessage = "Diện tích Villa phải từ 1 đến 100")]
    public int Sqft {set; get;}

    [Required]
    [Range(1, 100, ErrorMessage = "Số người ở Villa phải từ 1 đến 100")]
    public int Occupancy {set; get;}

    [DefaultValue(0)]
    public int Rate {set; get;}

    public string ImageUrl {set; get;}

    [DefaultValue("")]
    public string Amentity {set; get;}
}