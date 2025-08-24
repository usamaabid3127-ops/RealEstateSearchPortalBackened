using RealEstateSearchPortal.Models;

namespace RealEstateSearchPortal.Repository
{
    public interface IPropertyRepository
    {
        Task<List<Property>> GetFilteredProperties(PropertyFilter filter);
        Task<Property> GetPropertyById(int id);
        Task ToggleFavorite(int userId, int propertyId);
        Task<List<Property>> GetUserFavorites(int userId);
    }

    public class PropertyFilter
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Bedrooms { get; set; }
        public string? Suburb { get; set; }
        public ListingType? Type { get; set; }
    }
}
