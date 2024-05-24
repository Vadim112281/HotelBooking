using BookingHotel.Data;
using BookingHotel.Data.Entities;
using BookingHotel.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Services
{
    public class RoomTypeService
    {
        private readonly ApplicationDbContext _context;

        public RoomTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> CreateRoomTypeAsync(RoomTypeCreateModel model, string userId)
        {
            if (await _context.RoomTypes.AnyAsync(x => x.Name == model.Name))
            {
                return $"Room type with the same name {model.Name} already exists";
            }

            var roomType = new RoomType
            {
                Name = model.Name,
                AddedBy = userId,
                AddedOn = DateTime.Now,
                Description = model.Description,
                Image = model.Image,
                IsActive = model.IsActive,
                MaxAdults = model.MaxAdults,
                MaxChildren = model.MaxChildren,
                Price = model.Price,
            };

            await _context.RoomTypes.AddAsync(roomType);
            await _context.SaveChangesAsync();

            
        }
    }
}
