namespace VillaApi.Config;
using AutoMapper;
using VillaApi.Entities;
using VillaApi.DTOs;
public class MappingConfig:Profile
{
    public MappingConfig(){
        CreateMap<Villa, VillaDto>();
        CreateMap<VillaDto, Villa>();

        CreateMap<Villa, VillaCreateDto>().ReverseMap();

        CreateMap<VillaNumber, VillaNumberDto>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberCreateDto>().ReverseMap();
        CreateMap<VillaNumber, VillaNumberUpdateDto>().ReverseMap();


    }
}