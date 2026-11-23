
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstateSearchPortal.Data;
using RealEstateSearchPortal.Repository.RealEstatePortal.Repository;
using RealEstateSearchPortal.Repository;
using RealEstateSearchPortal.Services.RealEstatePortal.Services;
using RealEstateSearchPortal.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Properties.Any())
    {
        var sale = RealEstateSearchPortal.Models.ListingType.Sale;
        var rent = RealEstateSearchPortal.Models.ListingType.Rent;
        db.Properties.AddRange(
            new RealEstateSearchPortal.Models.Property
            {
                Title = "Modern 3-Bedroom House",
                Address = "Sector F-10, Islamabad",
                Price = 45000000m,
                Type = sale,
                Bedrooms = 3, Bathrooms = 3, CarSpots = 2,
                Description = "Bright modern house with a open kitchen and a small garden.",
                ImageUrlsJson = new List<string> { "https://picsum.photos/seed/house1/800/600" }
            },
            new RealEstateSearchPortal.Models.Property
            {
                Title = "Cozy 2-Bedroom Apartment",
                Address = "Bahria Town Phase 4, Rawalpindi",
                Price = 85000m,
                Type = rent,
                Bedrooms = 2, Bathrooms = 2, CarSpots = 1,
                Description = "Furnished apartment close to shops and parks.",
                ImageUrlsJson = new List<string> { "https://picsum.photos/seed/house2/800/600" }
            },
            new RealEstateSearchPortal.Models.Property
            {
                Title = "Spacious 5-Bedroom Villa",
                Address = "DHA Phase 2, Islamabad",
                Price = 120000000m,
                Type = sale,
                Bedrooms = 5, Bathrooms = 5, CarSpots = 3,
                Description = "Large villa with a lounge, a terrace and a servant quarter.",
                ImageUrlsJson = new List<string> { "https://picsum.photos/seed/house3/800/600" }
            },
            new RealEstateSearchPortal.Models.Property
            {
                Title = "Studio Apartment Near Metro",
                Address = "Saddar, Rawalpindi",
                Price = 40000m,
                Type = rent,
                Bedrooms = 1, Bathrooms = 1, CarSpots = 0,
                Description = "Compact studio, ten minutes walk from the metro station.",
                ImageUrlsJson = new List<string> { "https://picsum.photos/seed/house4/800/600" }
            },
            new RealEstateSearchPortal.Models.Property
            {
                Title = "Family Home with Garden",
                Address = "Gulraiz Housing Scheme, Rawalpindi",
                Price = 28500000m,
                Type = sale,
                Bedrooms = 4, Bathrooms = 3, CarSpots = 2,
                Description = "Quiet street, large garden and a separate guest room.",
                ImageUrlsJson = new List<string> { "https://picsum.photos/seed/house5/800/600" }
            }
        );
        db.SaveChanges();
    }
}
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
