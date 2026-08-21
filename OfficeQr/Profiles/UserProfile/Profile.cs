using AutoMapper;
using OfficeQr.Dtos.User;
using OfficeQr.Entity;

namespace OfficeQr.Profiles.UserProfile;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty));
    }
}
