using AutoMapper;
using OfficeQr.Dtos.Cabinet;
using OfficeQr.Entity;

namespace OfficeQr.Profiles.CabinetProfile;

public class CabinetProfile : Profile
{
    public CabinetProfile ()
    {
        CreateMap<Cabinet, Response>();
    }
}