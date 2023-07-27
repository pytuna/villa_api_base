#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VillaApi.Entities;

public class Villa
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {set; get;}

    [Required]
    [MaxLength(255)]
    public string Name {set; get;}

    public string Description {set; get;}

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

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

}