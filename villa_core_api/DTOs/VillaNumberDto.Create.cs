
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#nullable disable
namespace VillaApi.DTOs;

public class VillaNumberCreateDto
{
    // [Required] số nguyên dùng range ko dùng required để validate
    [Required]
    public int VillaNo { get; set; }
    [Required(ErrorMessage = "Tên {0} không được để trống")]
    public string SpecialDetail { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "{0} phải là số nguyên dương")]
    public int VillaID { get; set; }

}