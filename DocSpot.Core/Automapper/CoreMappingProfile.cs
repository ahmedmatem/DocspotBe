using AutoMapper;
using DocSpot.Core.Models;
using DocSpot.Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocSpot.Core.Automapper
{
    public class CoreMappingProfile : Profile
    {
        public CoreMappingProfile()
        {
            CreateMap<AppointmentDto, Appointment>().ReverseMap();
        }
    }
}
