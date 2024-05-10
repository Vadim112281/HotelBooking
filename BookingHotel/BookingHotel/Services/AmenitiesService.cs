using BookingHotel.Data;
using BookingHotel.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Services
{
    public interface IAmenitiesService
    {
        Task<List<Amenity>> GetAmenitiesAsync();
        Task<Amenity> SaveAmenityAsync(Amenity amenity);
    }

    public class AmenitiesService : IAmenitiesService
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
            if (amenity.Id == 0)
            {
                // Create new Amenity
                await context.Amenities.AddAsync(amenity);
            }
            else
            {
                // Update existing Amenity
                var dbEmenity = await context.Amenities
                                        .AsTracking()
                                        .FirstOrDefaultAsync(x => x.Id == amenity.Id);

                if (dbEmenity is null)
                {
                    throw new InvalidOperationException("Amenity does not exist");
                }

                dbEmenity.Name = amenity.Name;
                dbEmenity.Icon = amenity.Icon;
            }
            await context.SaveChangesAsync();
            return amenity;
        }
    }

    public class RoomService
    {

    }


}
