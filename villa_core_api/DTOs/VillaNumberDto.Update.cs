#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.DTOs;

public class VillaNumberUpdateDto
{
    [Required]
    public int VillaNo { get; set; }
    public string SpecialDetail { get; set; }
    public int VillaID { get; set; }

}