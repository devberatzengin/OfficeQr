using AutoMapper;
using OfficeQr.Dtos.Shelf;
using OfficeQr.Entity;

namespace OfficeQr.Profiles.ShelfProfile;


public class ShelfProfile : Profile
{

    public ShelfProfile ()
    {
        CreateMap<Shelf,Response>();
    }

} 