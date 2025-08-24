using AutoMapper;
using RealEstateSearchPortal.DTOs;
using RealEstateSearchPortal.Models;

namespace RealEstateSearchPortal.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyResponseDto>();
        }
    }
}
