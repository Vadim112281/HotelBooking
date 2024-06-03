using BookingHotel.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models
{
    public class RoomTypeCreateModel
    {

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Image { get; set; }


        [Required, Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }

        [Range(1, 10)]
        public int MaxAdults { get; set; }

        [Range(0, 10)]
        public int MaxChildren { get; set; }

        public bool IsActive { get; set; }

        public List<RoomTypeAmenityCreateModel> Amenities { get; set; } = new List<RoomTypeAmenityCreateModel>();

        public class RoomTypeAmenityCreateModel
        {
            public RoomTypeAmenityCreateModel(int id, int? unit) => (Id, Unit) = (id, unit);

            public int Id { get; set; }
            public int? Unit { get; set; }
        }

    }
}
