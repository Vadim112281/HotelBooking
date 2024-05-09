using BookingHotel.Data;
using BookingHotel.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Services
{
    public class AmenitiesService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public AmenitiesService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Amenity>> GetAmenitiesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Amenities.ToListAsync();
        }

        public async Task<Amenity> SaveAmenityAsync(Amenity amenity)
        {
            using var context = _contextFactory.CreateDbContext();
            if(amenity.Id == 0)
            {
                // Create new Amenity
                await context.Amenities.AddAsync(amenity);
            }
            else
            {
                // Update existing Amenity
            }
        }
    }

    public class RoomService
    {

    }


}
