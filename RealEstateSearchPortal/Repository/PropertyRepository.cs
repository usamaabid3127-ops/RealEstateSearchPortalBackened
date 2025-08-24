namespace RealEstateSearchPortal.Repository
{
    using Microsoft.EntityFrameworkCore;
    using RealEstateSearchPortal.Data;
    using RealEstateSearchPortal.Models;

    namespace RealEstatePortal.Repository
    {
        public class PropertyRepository : IPropertyRepository
        {
            private readonly AppDbContext _context;

            public PropertyRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<List<Property>> GetFilteredProperties(PropertyFilter filter)
            {
                var query = _context.Properties.AsQueryable();

                if (filter.MinPrice.HasValue)
                    query = query.Where(p => p.Price >= filter.MinPrice.Value);

                if (filter.MaxPrice.HasValue)
                    query = query.Where(p => p.Price <= filter.MaxPrice.Value);

                if (filter.Bedrooms.HasValue)
                    query = query.Where(p => p.Bedrooms == filter.Bedrooms.Value);

                if (!string.IsNullOrEmpty(filter.Suburb))
                    query = query.Where(p => p.Address.Contains(filter.Suburb));

                return await query.ToListAsync();
            }

            public async Task<Property> GetPropertyById(int id)
            {
                return await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);
            }

            public async Task ToggleFavorite(int userId, int propertyId)
            {
                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);

                if (existingFavorite != null)
                {
                    _context.Favorites.Remove(existingFavorite);
                }
                else
                {
                    var favorite = new Favorite
                    {
                        UserId = userId,
                        PropertyId = propertyId,
                        SavedAt = DateTime.UtcNow
                    };
                    await _context.Favorites.AddAsync(favorite);
                }

                await _context.SaveChangesAsync();
            }

            public async Task<List<Property>> GetUserFavorites(int userId)
            {
                return await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Include(f => f.Property)
                    .Select(f => f.Property)
                    .ToListAsync();
            }
        }
    }
}
