using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateSearchPortal.DTOs;
using RealEstateSearchPortal.Repository;

namespace RealEstateSearchPortal.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IMapper _mapper;

        [HttpGet]
        public async Task<IActionResult> GetProperties([FromQuery] PropertyFilter filter)
        {
            var properties = await _propertyRepo.GetFilteredProperties(filter);
            return Ok(_mapper.Map<List<PropertyResponseDto>>(properties));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _propertyRepo.GetPropertyById(id);
            return Ok(_mapper.Map<PropertyResponseDto>(property));
        }
    }
}
