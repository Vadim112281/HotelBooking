using BookingHotel.Data;
using BookingHotel.Data.Entities;
using BookingHotel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace BookingHotel.Services
{
    public interface IRoomTypeService
    {
        Task<bool?> SaveRoomTypeAsync(RoomTypeSaveModel model, string userId);
        Task<List<RoomTypeListModel>> GetRoomTypesForManagePageAsync();
        Task<RoomTypeSaveModel?> GetRoomTypeToEditAsync(int id);
        Task<RoomTypeSaveModel?> GetRoomTypeById(int id);
        Task<RoomType?> UpdateRoomTypeAsync(RoomType roomType, int id);
        Task<List<Room>> GetRoomAsync(int roomTypeId);
    }

    public class RoomTypeService : IRoomTypeService
    {
        private readonly ApplicationDbContext _context;

        public RoomTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool?> SaveRoomTypeAsync(RoomTypeSaveModel model, string userId)
        {
            RoomType? roomType;

            if(model.Id == 0)
            {
                if (await _context.RoomTypes.AnyAsync(x => x.Name == model.Name))
                {
                    //return $"Room type with the same name {model.Name} already exists";
                    return false;
                }

                roomType = new RoomType
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
            }
            else
            {
                if (await _context.RoomTypes.AnyAsync(x => x.Name == model.Name && x.Id != model.Id))
                {
                    //return $"Room type with the same name {model.Name} already exists";
                    return false;
                }

                roomType = await _context.RoomTypes.AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Id == model.Id);
                if(roomType is null)
                {
                    return false;
                }

                roomType.Name = model.Name;
                roomType.Description = model.Description;
                roomType.Image = model.Image;
                roomType.IsActive = model.IsActive;
                roomType.MaxAdults = model.MaxAdults;
                roomType.MaxChildren = model.MaxChildren;
                roomType.Price = model.Price;

                roomType.LastUpdatedby = userId;
                roomType.LastUpdatedOn = DateTime.Now;

                var existingRoomTypeAmenities = await _context.RoomTypeAmenities
                                                .Where(x => x.RoomTypeId == model.Id)
                                                .ToListAsync();
                _context.RoomTypeAmenities.RemoveRange(existingRoomTypeAmenities);
            }
            
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
    
        public async Task<List<RoomTypeListModel>> GetRoomTypesForManagePageAsync()
        {
            return await _context.RoomTypes
                                .Select(x => new RoomTypeListModel(x.Id, x.Name, x.Image, x.Price)).ToListAsync();
        }

        public async Task<RoomTypeSaveModel?> GetRoomTypeToEditAsync(int id)
        {
            var roomType = await _context.RoomTypes.Include(x => x.Amenities)
                                            .Where(x => x.Id == id)
                                            .Select(x => new RoomTypeSaveModel
                                            {
                                                Name = x.Name,
                                                Image = x.Image,
                                                Price = x.Price,
                                                Description = x.Description,
                                                IsActive = x.IsActive,
                                                Id = id,
                                                MaxAdults = x.MaxAdults,
                                                MaxChildren = x.MaxChildren,
                                                Amenities = x.Amenities.Select(a => new RoomTypeSaveModel.RoomTypeAmenitySaveModel(a.AmenityId, a.Unit)).ToList()
                                            })
                                            .FirstOrDefaultAsync();

            return roomType;
        }

        public async Task<RoomTypeSaveModel?> GetRoomTypeById(int id)
        {
            if(id != 0)
            {
                var roomType = await _context.RoomTypes.Include(x => x.Amenities)
                                   .Where(x => x.Id == id)
                                   .Select(x => new RoomTypeSaveModel
                                   {
                                       Name = x.Name,
                                       Image = x.Image,
                                       Price = x.Price,
                                       Description = x.Description,
                                       IsActive = x.IsActive,
                                       Id = id,
                                       MaxAdults = x.MaxAdults,
                                       MaxChildren = x.MaxChildren,
                                       Amenities = x.Amenities.Select(a => new RoomTypeSaveModel.RoomTypeAmenitySaveModel(a.AmenityId, a.Unit)).ToList()
                                   })
                                   .FirstOrDefaultAsync();

                return roomType;
            }
            else
            {
                return null;
            }
        }

        public async Task<RoomType?> UpdateRoomTypeAsync(RoomType roomType, int id)
        {
            var existingRoomType = await _context.RoomTypes.Include(x => x.Amenities)
                                   .Where(x => x.Id == id)
                                   .FirstOrDefaultAsync();

            existingRoomType.Image = roomType.Image;
            existingRoomType.Name = roomType.Name;
            existingRoomType.Price = roomType.Price;
            existingRoomType.Description = roomType.Description;
            existingRoomType.MaxAdults = roomType.MaxAdults;
            existingRoomType.MaxChildren = roomType.MaxChildren;
            existingRoomType.IsActive = roomType.IsActive;
            existingRoomType.Amenities = roomType.Amenities;
            existingRoomType.AddedOn = existingRoomType.AddedOn;
            existingRoomType.AddedBy = existingRoomType.AddedBy;
            existingRoomType.LastUpdatedOn = DateTime.Now;
            

             _context.RoomTypes.Update(existingRoomType);


            await _context.SaveChangesAsync();

            return existingRoomType;

        }

        public async Task<List<Room>> GetRoomAsync(int roomTypeId)
        {
            return await _context.Rooms
                                .Where(x => x.RoomTypeId == roomTypeId)
                                .ToListAsync(); ;

        }

        public async Task<MethodResult<Room>> SaveRoomAsync(Room room)
        {
            if(room.Id == 0)
            {
                // Creating
                _context.Rooms.Add(room);
            }
            else
            {
                // Updating
                var dbRoom = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == room.Id);
                if(dbRoom is null)
                {
                    return "Invalid request";
                }
            }
        }
    }
}
