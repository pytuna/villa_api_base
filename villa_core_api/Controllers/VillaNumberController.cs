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
public class VillaNumberController : ControllerBase
{
    protected readonly ApiResponse _apiResponse;
    private readonly VillaNumberService _villaNumberService;
    private readonly VillaService _villaService;

    private readonly ILogger<VillaNumberController> _logger;

    public VillaNumberController(VillaNumberService villaNumberService, VillaService villaService,ILogger<VillaNumberController> logger)
    {
        _villaNumberService = villaNumberService;
        _villaService = villaService;
        _logger = logger;
        _apiResponse = new();

    }
    [HttpGet(Name = "GetVillaNumbers")]
    [ProducesResponseType(typeof(IEnumerable<VillaNumberDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<VillaNumberDto>>> GetVillaNumbers([FromQuery] int limit = 5, [FromQuery] int offset = 0)
    {
        try
        {
            var villaNumbers = await _villaNumberService.GetVillaNumbersAsync(limit, offset);
            _apiResponse.Success(villaNumbers);
            return Ok(_apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.PushErrors(ex.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VillaNumberDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<VillaNumberDto>> GetVillaNumber([FromRoute] int id)
    {
        try
        {
            var villaNumber = await _villaNumberService.GetVillaNumberByIdAsync(id);
            if (villaNumber == null)
            {
                _apiResponse.PushErrors("VillaNumber not found");
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
            else
            {
                _apiResponse.Success(villaNumber);
                return Ok(_apiResponse);
            }
        }
        catch (Exception ex)
        {
            _apiResponse.PushErrors(ex.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
        }
    }

    [HttpPost(Name = "CreateVillaNumber")]
    [ProducesResponseType(typeof(VillaNumberDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<VillaNumberDto>> CreateVillaNumber([FromBody] VillaNumberCreateDto villaNumberCreateDto)
    {
        try
        {
            
            var villa = await _villaService.GetVillaByIdAsync(villaNumberCreateDto.VillaID);
            if(villa == null){
                _apiResponse.PushErrors("Villa not found");
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
            var villaCreated =  await _villaNumberService.CreateVillaNumberAsync(villaNumberCreateDto);
            _apiResponse.Success(villaCreated);
            return CreatedAtAction(nameof(GetVillaNumbers), new { id = villaCreated.VillaNo }, _apiResponse);
        }
        catch (Exception ex)
        {
            _apiResponse.PushErrors(ex.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(VillaNumberDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> DeleteVillaNumber([FromRoute] int id){
        try
        {
            bool isDeleted = await _villaNumberService.DeleteVillaNumberAsync(id);
            if(isDeleted){
                _apiResponse.Success(null, HttpStatusCode.NoContent);
                return Ok(_apiResponse);
            }
            else{
                _apiResponse.PushErrors("VillaNumber not found");
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
        }
        catch (System.Exception ex)
        {
            _apiResponse.PushErrors(ex.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(VillaNumberDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult> UpdateVillaNumber([FromRoute] int id, [FromBody] VillaNumberUpdateDto villaNumberUpdateDto){
        try
        {
            var villa = await _villaService.GetVillaByIdAsync(villaNumberUpdateDto.VillaID);
            if(villa == null){
                _apiResponse.PushErrors("Villa not found");
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
            bool isUpdated = await _villaNumberService.UpdateVillaNumberAsync(id, villaNumberUpdateDto);
            if(isUpdated){
                _apiResponse.Success(null, HttpStatusCode.NoContent);
                return Ok(_apiResponse);
            }
            else{
                _apiResponse.PushErrors("VillaNumber not found");
                _apiResponse.Fail(HttpStatusCode.NotFound);
                return NotFound(_apiResponse);
            }
        }
        catch (System.Exception ex)
        {
            _apiResponse.PushErrors(ex.Message);
            _apiResponse.Fail(HttpStatusCode.InternalServerError);
            return StatusCode((int)HttpStatusCode.InternalServerError, _apiResponse);
        }
    }

}