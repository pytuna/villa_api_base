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

public class VillaNumberService
{
    private readonly IMapper _mapper;
    private readonly IVillaNumberRepository _repository;

    public VillaNumberService(IMapper mapper, IVillaNumberRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<VillaNumberDto>> GetVillaNumbersAsync(int limit, int offet)
    {
        try
        {
            var villaNumbers = await _repository.GetAllAsync(limit, offet, null, null, nameof(VillaNumber.Villa));
            var dto = _mapper.Map<List<VillaNumberDto>>(villaNumbers);
            return dto;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public async Task<VillaNumberDto?> GetVillaNumberByIdAsync(int id)
    {
        try
        {
            var villaNumber = await _repository.GetOneAsync(x => x.VillaNo == id, false, nameof(VillaNumber.Villa));
            if(villaNumber == null){
                return null;
            }else{
                var dto = _mapper.Map<VillaNumberDto>(villaNumber);
                return dto;
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }
    public async Task<VillaNumberDto> CreateVillaNumberAsync(VillaNumberCreateDto villaNumberCreateDto)
    {
        try
        {
            VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDto);
            villaNumber.CreatedAt = DateTime.Now;
            villaNumber.UpdatedAt = DateTime.Now;
            await _repository.AddAsync(villaNumber);
            var dto = _mapper.Map<VillaNumberDto>(villaNumber);
            return dto;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteVillaNumberAsync(int villaNo){
        var villa = await _repository.GetOneAsync(x => x.VillaNo == villaNo);
        if(villa == null){
            return false;
        }else{
            await _repository.DeleteAsync(villa);
            return true;
        }
    }

    public async Task<bool> UpdateVillaNumberAsync(int villaNo, VillaNumberUpdateDto villaNumberUpdateDto){
        var villa = await _repository.GetOneAsync(x => x.VillaNo == villaNo);
        if(villa == null){
            return false;
        }else{
            villa.UpdatedAt = DateTime.Now;
            _mapper.Map(villaNumberUpdateDto, villa);
            await _repository.UpdateAsync(villa);
            return true;
        }
    }
}