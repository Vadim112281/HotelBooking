using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingHotel.Data.Entities
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Image { get; set; }

        [Required, Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AddedOn { get; set; }
        public string AddedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string? LastUpdatedby { get; set; }

        [ForeignKey(nameof(AddedBy))]
        public virtual ApplicationUser AddedByUser { get; set; }
    }
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string RoomNumber { get; set; }
    }

    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string Name { get; set; }

        [Required, MaxLength(25)]
        public string Icon { get; set; }
    }

    public class RoomAmenity
    {
        public int RoomId { get; set; }
        public int AmenityId { get; set; }

        public int? Unit { get; set; }

        public virtual Room Room { get; set; }
        public virtual Amenity Amenity { get; set; }
    }

}
