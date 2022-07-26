using Identity.Models;
using AutoMapper;

namespace Identity.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<SignupRequest, UserModel>();
        }
    }
}