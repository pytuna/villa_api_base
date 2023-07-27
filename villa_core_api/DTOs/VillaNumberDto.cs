#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.DTOs;

public class VillaNumberDto
{
    [Required]
    public int VillaNo { get; set; }
    public string SpecialDetail { get; set; }
    public VillaDto Villa {set; get;}
}