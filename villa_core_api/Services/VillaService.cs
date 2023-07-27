using Microsoft.AspNetCore.Mvc;
using VillaApi.Entities;
using VillaApi.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using VillaApi.Repositories;
using VillaApi.Interfaces;

namespace VillaApi.Services;

public class VillaService
{
    private readonly IMapper _mapper;
    private readonly IVillaRepository _repository;
    public VillaService( IMapper mapper, IVillaRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<VillaDto>> GetVillasAsync(int limit, int offset)
    {
        try
        {
            var villas = await _repository.GetAllAsync(limit, offset);
            var villaDtos = _mapper.Map<List<VillaDto>>(villas);
            return villaDtos;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<VillaDto?> GetVillaByIdAsync(int id)
    {
        try
        {
            Villa? villa = await _repository.GetOneAsync(v => v.Id == id);

            if (villa == null)
            {
                return null;
            }
            else
            {
                return _mapper.Map<VillaDto>(villa);
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<bool> CheckVillaNameExist(string name)
    {
        var villa = await _repository.GetOneAsync(v => v.Name == name);
        // var villa = await query.FirstOrDefaultAsync();
        if (villa == null) return false;
        else return true;
    }

    public async Task<VillaDto> CreateVillaAsync(VillaCreateDto villaDto)
    {

        Villa villa = _mapper.Map<Villa>(villaDto);
        villa.CreatedAt = DateTime.Now;
        villa.UpdatedAt = DateTime.Now;

        await _repository.AddAsync(villa);
        var villaCreated = _mapper.Map<VillaDto>(villa);
        return villaCreated;
    }

    public async Task<bool> DeleteVillaAsync(int id)
    {
        try
        {
            var villa = await _repository.GetOneAsync(v => v.Id == id);
            if (villa == null) return false;
            await _repository.DeleteAsync(villa);
            return true;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<bool> UpdatedVillaAsyncc(int id, VillaCreateDto villaDto)
    {
        try
        {
            var villa = await _repository.GetOneAsync(v => v.Id == id);

            if (villa == null) return false;

            _mapper.Map(villaDto, villa);

            villa.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(villa);
            return true;
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public async Task<bool> UpdatedDynamicFieldVilla(int id, JsonPatchDocument<VillaCreateDto> patchVillaDto, ModelStateDictionary ModelState)
    {
        var villa = await _repository.GetOneAsync(v => v.Id == id);

        // _context.Villas.AsNoTracking(); Không track sự thay đổi của villa nên savechange không thay đổi
        if (villa == null) return false;
        var villaCreateDto = new VillaCreateDto()
        {
            Name = villa.Name,
            Sqft = villa.Sqft,
            Occupancy = villa.Occupancy,
            Description = villa.Description,
            ImageUrl = villa.ImageUrl,
            Amentity = villa.Amentity,
            Rate = villa.Rate
        };

        patchVillaDto.ApplyTo(villaCreateDto, ModelState);

        ValidationContext context = new ValidationContext(villaCreateDto, null, null);
        List<ValidationResult> results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(villaCreateDto, context, results, true);
        if (!isValid)
        {
            foreach (var validationResult in results)
            {
                string[] memberNames = validationResult.MemberNames.ToArray();
                ModelState.AddModelError(memberNames[0], validationResult.ErrorMessage ?? "Lỗi bất định");
                System.Console.WriteLine(validationResult.ErrorMessage);
            }
            return false;
        }

        _mapper.Map(villaCreateDto, villa);

        await _repository.UpdateAsync(villa);
        return true;
    }
}