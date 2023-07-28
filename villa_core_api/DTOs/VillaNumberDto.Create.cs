#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.DTOs;

public class VillaNumberCreateDto
{
    [Required]
    public int VillaNo { get; set; }
    public string SpecialDetail { get; set; }
    
    [Required(ErrorMessage = "ID Villa không được để trống")]
    public int VillaID { get; set; }

}