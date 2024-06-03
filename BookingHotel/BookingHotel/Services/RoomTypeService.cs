using BookingHotel.Data;
using BookingHotel.Data.Entities;
using BookingHotel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace BookingHotel.Services
{
    public interface IRoomTypeService
    {
        Task<bool?> CreateRoomTypeAsync(RoomTypeCreateModel model, string userId);
    }

    public class RoomTypeService : IRoomTypeService
    {
        private readonly ApplicationDbContext _context;

        public RoomTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool?> CreateRoomTypeAsync(RoomTypeCreateModel model, string userId)
        {
            if (await _context.RoomTypes.AnyAsync(x => x.Name == model.Name))
            {
                //return $"Room type with the same name {model.Name} already exists";
                return false;
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

            if (model.Amenities.Count > 0)
            {
                var roomTypeAmenities = model.Amenities.Select(x => new RoomTypeAmenity
                {
                    AmenityId = x.Id,
                    RoomTypeId = roomType.Id,
                    Unit = x.Unit
                });
                await _context.RoomTypeAmenities.AddRangeAsync(roomTypeAmenities);
                await _context.SaveChangesAsync();
            }

            //return roomType.Id.ToString();
            return true;
        }
    }
}
