using AutoMapper;
using Medius.Model;
using Medius.Model.ViewModels;

namespace Medius
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, AccountResponse>();

            CreateMap<ApplicationUser, AuthenticateResponse>();

            CreateMap<RegisterRequest, ApplicationUser>();

            CreateMap<CreateRequest, ApplicationUser>();

            CreateMap<UpdateRequest, ApplicationUser>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
        }
    }
}
