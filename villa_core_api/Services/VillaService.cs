using Microsoft.AspNetCore.Mvc;
using VillaApi.Models;
using VillaApi.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using VillaApi.Repositories;

namespace VillaApi.Services;

public class VillaService
{
    private readonly ModelAppContext _context;
    private readonly IMapper _mapper;
    private readonly VillaRepository _repository;
    public VillaService(ModelAppContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _repository = new VillaRepository(_context);
    }

    public async Task<List<VillaDto>> GetVillas(int limit, int offset)
    {
        var villas = await _repository.GetAllVillasAsync(limit, offset);
        var villaDtos = _mapper.Map<List<VillaDto>>(villas);
        return villaDtos;
    }

    public async Task<VillaDto?> GetVillaById(int id)
    {
        Villa? villa = await _repository.GetVillaByIdAsync(v => v.Id == id);

        if (villa == null)
        {
            return null;
        }
        else
        {
            return _mapper.Map<VillaDto>(villa);
        }
    }

    public async Task<bool> CheckVillaNameExist(string name)
    {
        var query = from Villas in _context.Villas
                    where Villas.Name.ToLower() == name.ToLower()
                    select Villas;
        var villa = await query.FirstOrDefaultAsync();
        if (villa == null) return false;
        else return true;
    }

    public async Task CreateVilla(VillaCreateDto villaDto)
    {

        Villa villa = new Villa()
        {
            Name = villaDto.Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Sqft = villaDto.Sqft,
            Occupancy = villaDto.Occupancy,
            Description = villaDto.Description,
            ImageUrl = villaDto.ImageUrl,
            Amentity = villaDto.Amentity,
            Rate = villaDto.Rate
        };

        await _context.Villas.AddAsync(villa);
        await _context.SaveChangesAsync();
        // var villaCreated = new VillaDto{
        //     Id = villa.Id,
        //     Name = villa.Name,
        //     Sqft = villa.Sqft,
        //     Occupancy = villa.Occupancy,
        //     Description = villa.Description,
        //     ImageUrl = villa.ImageUrl,
        //     Amentity = villa.Amentity,
        //     Rate = villa.Rate
        // };
        // System.Console.WriteLine("\n\n DKMDKMDKMDKMDKM");
        // return villaCreated;
    }

    public async Task<bool> DeleteVilla(int id)
    {
        var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
        if (villa == null) return false;
        _context.Villas.Remove(villa);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatedVilla(int id, VillaCreateDto villaDto)
    {
        var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
        if (villa == null) return false;

        _mapper.Map(villaDto, villa);
        villa.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatedDynamicFieldVilla(int id, JsonPatchDocument<VillaCreateDto> patchVillaDto, ModelStateDictionary ModelState)
    {
        var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
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
        villa.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        
        return true;
    }
}