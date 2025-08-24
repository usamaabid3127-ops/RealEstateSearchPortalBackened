using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateSearchPortal.DTOs;
using RealEstateSearchPortal.Repository;
using RealEstateSearchPortal.Repository.RealEstatePortal.Repository;

namespace RealEstateSearchPortal.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public PropertiesController(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties([FromQuery] PropertyFilter filter)
        {
            var properties = await _propertyRepository.GetFilteredProperties(filter);
            // var result = _mapper.Map<List<PropertyResponseDto>>(properties);
            var result = properties.Select(p => new PropertyResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Address = p.Address,
                Price = p.Price,
                Type = p.Type,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                CarSpots = p.CarSpots,
                Description = p.Description,
                ImageUrlsJson = p.ImageUrlsJson
            }).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(int id)
        {
            var property = await _propertyRepository.GetPropertyById(id);
            if (property == null)
                return NotFound();

            var result = _mapper.Map<PropertyResponseDto>(property);
            return Ok(result);
        }
    }
}
