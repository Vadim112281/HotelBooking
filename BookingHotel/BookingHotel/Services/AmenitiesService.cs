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
        Task UpdateAmenityAsync(Amenity amenity, int id);
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
            var amenities = await  _context.Amenities.ToListAsync();

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

        public async Task UpdateAmenityAsync(Amenity amenity, int id)
        {
            var existingAmenity = await _context.Amenities.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAmenity == null)
            {
                throw new InvalidOperationException("Amenity not found");
            }

            existingAmenity.Name = amenity.Name;
            existingAmenity.Icon = amenity.Icon;

            await _context.SaveChangesAsync();
        }


        public async Task<bool> DeleteAmenityAsync(int id)
        {
            var amenityToDelete = await _context.Amenities.FindAsync(id);

            if(amenityToDelete is not null)
            {
                 _context.Amenities.Remove(amenityToDelete);
                 await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
