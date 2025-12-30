namespace DocSpot.WebAPI.Automapper
{
    using AutoMapper;

    using DocSpot.Core.Models;
    using DocSpot.Infrastructure.Data.Models;

    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            CreateMap<AppointmentViewModel, AppointmentDto>().ReverseMap();
        }
    }
}
