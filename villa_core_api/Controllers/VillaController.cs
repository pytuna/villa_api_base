using Microsoft.AspNetCore.Mvc;
using VillaApi.Services;
using VillaApi.Models;

using VillaApi.DTOs;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.JsonPatch;
using System.Net;

namespace VillaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VillaController : ControllerBase
{
    protected readonly ApiResponse _apiResponse;
    private readonly VillaService _villaService;
    private readonly ILogger<VillaController> _logger;

    public VillaController(VillaService villaService, ILogger<VillaController> logger)
    {
        _villaService = villaService;
        _logger = logger;
        _apiResponse = new();
    }

    /// <param name="limit">Số lượng Villa cần lấy</param>
    /// <param name="offset">Vị trí bắt đầu lấy</param>
    [SwaggerOperation(Summary = "Lấy danh sách Villa")]
    [HttpGet(Name = "GetVillas")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VillaDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
    public async Task<ActionResult<IEnumerable<VillaDto>>> GetAll([FromQuery] int limit = 5, [FromQuery] int offset = 0)
    {
        try
        {
            var villas = await _villaService.GetVillasAsync(limit, offset);
            _apiResponse.Success(villas);
            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.PushErrors(e.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
        }
    }

    /// <param name="id">Id của Villa</param>
    [SwaggerOperation(Summary = "Lấy Villa theo Id")]
    [HttpGet("{id:int}", Name = "GetVillaById")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VillaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
    public async Task<ActionResult<VillaDto>> GetById(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.Fail(HttpStatusCode.BadRequest);
                return BadRequest(_apiResponse);
            }
            var villa = await _villaService.GetVillaByIdAsync(id);
            if (villa == null)
            {
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
            else
            {
                _apiResponse.Success(villa);
                return Ok(_apiResponse);
            }
        }
        catch (Exception e)
        {
            _apiResponse.PushErrors(e.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
        }
    }

    /// <param name="villaCreateDto">Villa cần tạo</param>
    [SwaggerOperation(Summary = "Tạo Villa mới")]
    [HttpPost(Name = "CreateVilla")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(VillaCreateDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
    public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto villaCreateDto)
    {
        try
        {
            if (villaCreateDto == null)
            {
                _apiResponse.Fail(HttpStatusCode.BadRequest);
                return BadRequest(_apiResponse);
            }
            if (await _villaService.CheckVillaNameExist(villaCreateDto.Name))
            {
                _apiResponse.PushErrors("Villa đã tồn tại");
                _apiResponse.Fail(HttpStatusCode.BadRequest);
                return BadRequest(_apiResponse);
            }
            var villa = await _villaService.CreateVillaAsync(villaCreateDto);
            // dùng để tạo tài nguyên mới mà action GetById trả về
            // ví dụ khi tạo thành công sẽ phát sinh trong Header response Location: https://localhost:7142/api/villa/161
            _apiResponse.Success(villa);
            return CreatedAtAction(nameof(GetById), new { id = villa.Id }, _apiResponse);
            // return CreatedAtRoute("GetVillaById", new { id = villa.Id }, villa);
        }
        catch (Exception e)
        {
            _apiResponse.PushErrors(e.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
        }
    }

    /// <param name="id">Id của Villa</param>
    [SwaggerOperation(Summary = "Xóa Villa theo Id")]
    [HttpDelete("{id:int}", Name = "DeletaVillaById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
    public async Task<ActionResult> DeleteVilla([FromRoute]int id)
    {
        try
        {
            if (id <= 0)
            {
                _apiResponse.PushErrors("Id không được bằng 0");
                _apiResponse.Fail(HttpStatusCode.BadRequest);
                return BadRequest(_apiResponse);
            }
            var isVillaDeleted = await _villaService.DeleteVillaAsync(id);
            if (isVillaDeleted)
            {
                _apiResponse.Success(null, HttpStatusCode.NoContent);
                return Ok(_apiResponse);
            }
            else
            {
                _apiResponse.PushErrors("Villa không tồn tại");
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
        }
        catch (Exception e)
        {
            _apiResponse.PushErrors(e.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
        }
    }


    /// <param name="id">Id của Villa</param>
    /// <param name="villaCreateDto">Villa cần cập nhật</param>
    [SwaggerOperation(Summary = "Cập nhật Villa theo Id")]
    [HttpPut("{id:int}", Name = "UpdateVillaById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
    public async Task<ActionResult> UpdateById([FromRoute] int id, [FromBody] VillaCreateDto villaCreateDto)
    {
        try
        {
            if (id <= 0 || villaCreateDto == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.PushErrors("Id không được bằng 0");   
                return BadRequest(_apiResponse);
            }
            if (await _villaService.CheckVillaNameExist(villaCreateDto.Name))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.PushErrors("Villa đã tồn tại");
                return BadRequest(_apiResponse);
            }
            var isVillaUpdated = await _villaService.UpdatedVillaAsyncc(id, villaCreateDto);
            if (isVillaUpdated)
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
            }
            else
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.PushErrors("Không tìm thấy Villa cần cập nhật");
                return NotFound(_apiResponse);
            }
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.PushErrors(e.Message);
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
        }
        return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
    }

    [HttpPatch("{id:int}", Name = "UpdateDynamicFieldVillaById")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
    public async Task<ActionResult> UpdateDynamicFieldById([FromRoute] int id, [FromBody] JsonPatchDocument<VillaCreateDto> villaUpdateDto)
    {
        try
        {
            if (id <= 0 || villaUpdateDto == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.PushErrors("Id không được bằng 0");   
                return BadRequest(_apiResponse);
            }
            VillaCreateDto villaDto = new();
            villaUpdateDto.ApplyTo(villaDto);

            if (villaDto.Name != "" && villaDto.Name != null &&  await _villaService.CheckVillaNameExist(villaDto.Name))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.PushErrors("Villa đã tồn tại");
                return BadRequest(_apiResponse);
            }

            bool isVillaUpdated = await _villaService.UpdatedDynamicFieldVilla(id, villaUpdateDto, ModelState);

            if (!ModelState.IsValid)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                ModelState.Values.SelectMany(x => x.Errors).ToList().ForEach(x => _apiResponse.PushErrors(x.ErrorMessage));
                return BadRequest(_apiResponse);
            }

            if (isVillaUpdated)
            {
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
            }
            else
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.PushErrors("Không tìm thấy Villa cần cập nhật");
                return NotFound(_apiResponse);
            }
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.PushErrors(e.Message);
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
        }
        return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
    }
}