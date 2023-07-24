using Microsoft.AspNetCore.Mvc;
using VillaApi.Models;
using VillaApi.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace VillaApi.Services;

public class VillaService
{
    private readonly ModelAppContext _context;
    public VillaService(ModelAppContext context)
    {
        _context = context;
    }

    public List<VillaDto> GetVillas(int limit, int offset)
    {
        var villas = _context.Villas.Skip(limit * offset).Take(limit).ToList();

        var villaDtos = villas.Select(villa =>
        {
            return new VillaDto()
            {
                Id = villa.Id,
                Name = villa.Name,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy
            };
        }).ToList();

        return villaDtos;
    }

    public VillaDto? GetVillaById(int id)
    {
        var villa = _context.Villas.Find(id);

        if (villa == null)
        {
            return null;
        }
        else
        {
            return new VillaDto()
            {
                Id = villa.Id,
                Name = villa.Name,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy
            };
        }
    }

    public bool CheckVillaNameExist(string name)
    {
        var query = from Villas in _context.Villas
                    where Villas.Name.ToLower() == name.ToLower()
                    select Villas;
        var villa = query.FirstOrDefault();
        if (villa == null) return false;
        else return true;
    }

    public VillaDto CreateVilla(VillaCreateDto villaDto)
    {
        var villa = new Villa()
        {
            Name = villaDto.Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Sqft = villaDto.Sqft,
            Occupancy = villaDto.Occupancy
        };

        _context.Villas.Add(villa);
        _context.SaveChanges();
        var villaCreated = new VillaDto()
        {
            Id = villa.Id,
            Name = villa.Name,
            Sqft = villa.Sqft,
            Occupancy = villa.Occupancy
        };
        return villaCreated;
    }

    public bool DeleteVilla(int id)
    {
        var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null) return false;
        _context.Villas.Remove(villa);
        _context.SaveChanges();
        return true;
    }

    public bool UpdatedVilla(int id, VillaCreateDto villaDto)
    {
        var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null) return false;
        villa.Name = villaDto.Name;
        villa.UpdatedAt = DateTime.Now;
        villa.Sqft = villaDto.Sqft;
        villa.Occupancy = villaDto.Occupancy;
        _context.SaveChanges();
        return true;
    }

    public bool UpdatedDynamicFieldVilla(int id, JsonPatchDocument<VillaCreateDto> patchVillaDto, ModelStateDictionary ModelState)
    {
        var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null) return false;
        var villaDto = new VillaCreateDto()
        {
            Name = villa.Name,
            Sqft = villa.Sqft,
            Occupancy = villa.Occupancy
        };
        
        
        

        patchVillaDto.ApplyTo(villaDto, ModelState);

        ValidationContext context = new ValidationContext(villaDto, null, null);
        List<ValidationResult> results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(villaDto, context, results, true);
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
        villa.Name = villaDto.Name;
        villa.UpdatedAt = DateTime.Now;
        villa.Sqft = villaDto.Sqft;
        villa.Occupancy = villaDto.Occupancy;
        _context.SaveChanges();
        return true;
    }
}