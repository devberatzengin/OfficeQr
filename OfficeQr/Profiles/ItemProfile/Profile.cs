using System.IO.Compression;
using AutoMapper;
using OfficeQr.Dtos.Item;
using OfficeQr.Entity;

namespace OfficeQr.Profiles.ItemProfile;


public class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Item, Response>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.QrCode, opt => opt.MapFrom(src => src.QrCode))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ShelfId, opt => opt.MapFrom(src => src.ShelfId)); 
        
    }
}