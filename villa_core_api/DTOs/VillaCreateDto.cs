using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.DTOs;
#nullable disable

public class VillaCreateDto
{
    [Required(ErrorMessage = "Tên Villa không được để trống")]
    [MinLength(7, ErrorMessage = "Tên Villa tối thiểu 7 ký tự")]
    [MaxLength(255, ErrorMessage = "Tên Villa tối đa 255 ký tự")]
    // [RegularExpression(@"Villa", ErrorMessage = "Tên Villa phải chứa chuỗi Villa")]
    public string Name { set; get; }
    
    [Required]
    [Range(1, 1000, ErrorMessage = "Diện tích Villa phải từ 1 đến 100")]
    public int Sqft {set; get;}

    [Required]
    [Range(1, 100, ErrorMessage = "Số người ở Villa phải từ 1 đến 100")]
    public int Occupancy {set; get;}
}