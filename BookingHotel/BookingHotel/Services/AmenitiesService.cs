using BookingHotel.Data;
using BookingHotel.Data.Entities;
using BookingHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Services
{
    public interface IAmenitiesService
    {
        Task<List<Amenity>> GetAmenitiesAsync();
        Task<Amenity?> AddAmenityAsync(Amenity amenity);
        Task<Amenity?> UpdateAmenityAsync(Amenity amenity, int id);
        Task<Amenity?> GetAmenitySingleAsync(int id);
        Task<bool> DeleteAmenityAsync(int id);
    }

    public class AmenitiesService : IAmenitiesService
    {
        private readonly ApplicationDbContext _context;

        public AmenitiesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Amenity>> GetAmenitiesAsync()
        {
            var amenities = await  _context.Amenities.Where(x => !x.IsDeleted).ToListAsync();

            return amenities;   
        }

        public async Task<Amenity?> AddAmenityAsync(Amenity amenity)
        {
            // Checking on existing amenity
            var existingAmenity = await _context.Amenities.FirstOrDefaultAsync(x => x.Name == amenity.Name);

            if(existingAmenity is not null)
            {
                return null;
            }
            else
            {
                await _context.Amenities.AddAsync(amenity);
                await _context.SaveChangesAsync();

                return amenity;
            }
        }

        public async Task<Amenity?> UpdateAmenityAsync(Amenity amenity, int id)
        {
            var existingAmenity = await _context.Amenities.FirstOrDefaultAsync(x => x.Id == id);

            if(existingAmenity == null)
            {
                return null;
            }
            else
            {
                existingAmenity.Name = amenity.Name;
                existingAmenity.Icon = amenity.Icon;

                _context.Amenities.Update(existingAmenity);
                await _context.SaveChangesAsync();

                return amenity;
            }
        }

        public async Task<Amenity?> GetAmenitySingleAsync(int id)
        {
            var amenity = await _context.Amenities.FirstOrDefaultAsync(x => x.Id == id);

            if(amenity is null)
            {
                return null;
            }

            return amenity;
        }

        public async Task<bool> DeleteAmenityAsync(int id)
        {
            var amenityForDeleting = await  _context.Amenities.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(amenityForDeleting is not null)
            {
                amenityForDeleting.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }

    public class RoomService
    {

    }


}
