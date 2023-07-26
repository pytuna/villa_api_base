using Microsoft.AspNetCore.Mvc;
using VillaApi.Services;
using VillaApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.JsonPatch;

namespace VillaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VillaController : ControllerBase
{
    private readonly VillaService _villaService;
    private readonly ILogger<VillaController> _logger;

    public VillaController(VillaService villaService, ILogger<VillaController> logger)
    {
        _villaService = villaService;
        _logger = logger;
    }

    /// <param name="limit">Số lượng Villa cần lấy</param>
    /// <param name="offset">Vị trí bắt đầu lấy</param>
    [SwaggerOperation(Summary = "Lấy danh sách Villa")]
    [HttpGet(Name = "GetVillas")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VillaDto>))]
    public async Task<ActionResult<IEnumerable<VillaDto>>> Get([FromQuery] int limit = 5, [FromQuery] int offset = 0)
    {
        var villas = await _villaService.GetVillas(limit, offset);
        return Ok(villas);
    }

    /// <param name="id">Id của Villa</param>
    [SwaggerOperation(Summary = "Lấy Villa theo Id")]
    [HttpGet("{id:int}", Name = "GetVillaById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDto>> GetById(int id)
    {
        if (id == 0)
        {
            _logger.LogError("Id không được bằng 0");
            return BadRequest();
        }
        var villa = await _villaService.GetVillaById(id);
        if (villa == null)
        {
            return NotFound();
        }
        else
        {
            _logger.LogInformation("Lấy Villa thành công");
            return Ok(villa);
        }
    }

    /// <param name="villaCreateDto">Villa cần tạo</param>
    [SwaggerOperation(Summary = "Tạo Villa mới")]
    [HttpPost(Name = "CreateVilla")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VillaCreateDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDto>> Post([FromBody] VillaCreateDto villaCreateDto)
    {

        if (villaCreateDto == null)
        {
            return BadRequest();
        }
        if (await _villaService.CheckVillaNameExist(villaCreateDto.Name))
        {
            ModelState.AddModelError("CustomError", "Villa đã tồn tại");
            return BadRequest(ModelState);
        }
        var villa = _villaService.CreateVilla(villaCreateDto);
        // dùng để tạo tài nguyên mới mà action GetById trả về
        // ví dụ khi tạo thành công sẽ phát sinh trong Header response Location: https://localhost:7142/api/villa/161
        return CreatedAtAction(nameof(GetById), new { id = villa.Id }, villa);
        // return CreatedAtRoute("GetVillaById", new { id = villa.Id }, villa);
    }

    /// <param name="id">Id của Villa</param>
    [SwaggerOperation(Summary = "Xóa Villa theo Id")]
    [HttpDelete("{id:int}", Name = "DeletaVillaById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var isVillaDeleted = await _villaService.DeleteVilla(id);
        if (isVillaDeleted)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }


    /// <param name="id">Id của Villa</param>
    /// <param name="villaCreateDto">Villa cần cập nhật</param>
    [SwaggerOperation(Summary = "Cập nhật Villa theo Id")]
    [HttpPut("{id:int}", Name = "UpdateVillaById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateById([FromRoute] int id, [FromBody] VillaCreateDto villaCreateDto)
    {
        if (id <= 0 || villaCreateDto == null)
        {
            return BadRequest();
        }
        if (await _villaService.CheckVillaNameExist(villaCreateDto.Name))
        {
            ModelState.AddModelError("CustomError", "Villa đã tồn tại");
            return BadRequest(ModelState);
        }
        var isVillaUpdated = await _villaService.UpdatedVilla(id, villaCreateDto);
        if (isVillaUpdated)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPatch("{id:int}", Name = "UpdateDynamicFieldVillaById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateDynamicFieldById([FromRoute] int id, [FromBody] JsonPatchDocument<VillaCreateDto> villaUpdateDto)
    {
        if (id <= 0 || villaUpdateDto == null)
        {
            return BadRequest();
        }

        bool isVillaUpdated = await _villaService.UpdatedDynamicFieldVilla(id, villaUpdateDto, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (isVillaUpdated)
        {
            return NoContent();
        }
        else
        {

            return NotFound(ModelState);
        }
    }
}