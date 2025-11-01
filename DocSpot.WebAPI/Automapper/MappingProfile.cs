namespace DocSpot.WebAPI.Automapper
{
    using AutoMapper;

    using DocSpot.Core.Models;
    using DocSpot.Infrastructure.Data.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppointmentModel, Appointment>().ReverseMap();
        }
    }
}
