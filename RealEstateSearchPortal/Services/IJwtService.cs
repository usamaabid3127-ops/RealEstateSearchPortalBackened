namespace RealEstateSearchPortal.Services
{
    public interface IJwtService
    {
        string GenerateToken(int userId);
    }
}
