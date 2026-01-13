using AutoMapper;
using DocSpot.Core.Models;
using DocSpot.Core.Helpers;
using DocSpot.Infrastructure.Data.Models;

namespace DocSpot.Core.Automapper
{
    public class CoreMappingProfile : Profile
    {
        public CoreMappingProfile()
        {
            CreateMap<Appointment, AdminAppointmentDto>();

            CreateMap<AppointmentDto, Appointment>()
                .ForMember(d => d.PublicTokenHash, opt => opt.MapFrom(s =>
                    string.IsNullOrEmpty(s.PublicToken) 
                        ? null
                        : TokenHelper.ComputeSha256Hash(s.PublicToken)
                ))
                .ForMember(d => d.CancelTokenHash, opt => opt.MapFrom(s =>
                      string.IsNullOrEmpty(s.CancelToken)
                          ? null
                          : TokenHelper.ComputeSha256Hash(s.CancelToken)
                  ))
                .ReverseMap()
                .ForMember(d => d.PublicToken, opt => opt.Ignore())
                .ForMember(d => d.CancelToken, opt => opt.Ignore());

            CreateMap<Appointment, AppointmentPublicDto>();
        }
    }
}
