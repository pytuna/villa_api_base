namespace VillaApi.Config;
using AutoMapper;
using VillaApi.Models;
using VillaApi.DTOs;
public class MappingConfig:Profile
{
    public MappingConfig(){
        CreateMap<Villa, VillaDto>();
        CreateMap<VillaDto, Villa>();

        CreateMap<Villa, VillaCreateDto>().ReverseMap();
        CreateMap<VillaCreateDto, Villa>().ReverseMap();
    }
}