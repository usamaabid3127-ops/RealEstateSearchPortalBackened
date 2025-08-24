using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSearchPortal.DTOs;
using RealEstateSearchPortal.Repository;
using AutoMapper;

namespace RealEstateSearchPortal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IMapper _mapper;

        [HttpPost("{propertyId}")]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            await _propertyRepo.ToggleFavorite(userId, propertyId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var favorites = await _propertyRepo.GetUserFavorites(userId);
            return Ok(_mapper.Map<List<PropertyResponseDto>>(favorites));
        }
    }
}
