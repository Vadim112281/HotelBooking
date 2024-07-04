namespace BookingHotel.Models
{
    public record PageResult<TData>(int TotalCount, List<TData> Records);
}
