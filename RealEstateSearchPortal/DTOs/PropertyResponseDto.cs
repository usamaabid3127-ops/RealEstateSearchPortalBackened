using RealEstateSearchPortal.Models;

namespace RealEstateSearchPortal.DTOs
{
    public class PropertyResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public ListingType Type { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int CarSpots { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrlsJson { get; set; }
    }
}