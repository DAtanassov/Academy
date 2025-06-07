using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public class Reservation : BaseModel
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public RoomStatus Status { get; set; }
        public DateTime CreationDate { get; set; }

        public Reservation() { }
        
        public Reservation(int userId, Room room, DateTime checkInDate, DateTime checkOutDate)
        {
            this.UserId = userId;
            this.HotelId = room.HotelId;
            this.RoomId = room.Id;
            this.RoomTypeId = room.RoomTypeId;
            this.CheckInDate = checkInDate;
            this.CheckOutDate = checkOutDate;
            this.TotalPrice = room.PricePerNight * (decimal)(checkOutDate - checkInDate).TotalDays;
            this.Status = RoomStatus.booked;
            this.CreationDate = DateTime.Now;
        }
        public Reservation(int userId, Room room, DateTime checkInDate, DateTime checkOutDate, RoomStatus status)
        {
            this.UserId = userId;
            this.HotelId = room.HotelId;
            this.RoomId = room.Id;
            this.RoomTypeId = room.RoomTypeId;
            this.CheckInDate = checkInDate;
            this.CheckOutDate = checkOutDate;
            this.TotalPrice = room.PricePerNight * (decimal)(checkOutDate - checkInDate).TotalDays;
            this.Status = status;
            this.CreationDate = DateTime.Now;
        }

        public override string Info()
        {
            Hotel? hotel = (new HotelHelper()).GetHotelById(HotelId);
            Room? room = RoomHelper.GetRoomById(RoomId, HotelId);

            return $"Reservation ID: {Id}, " +
                   ((hotel == null) ? "" : $"Hotel ID: {hotel.ShortInfo()}, ") +
                   ((room == null) ? "" : $"Room ID: {room.ShortInfo()}, ") +
                   $"Check-in: {CheckInDate.ToShortDateString()}, " +
                   $"Check-out: {CheckOutDate.ToShortDateString()}, " +
                   $"Total price: {TotalPrice}," +
                   $"Status: {Enum.GetName(Status.GetType(), Status)}";
        }

        public override string ShortInfo()
            => $"Reserv. ID: {Id}, status: {Enum.GetName(Status.GetType(), Status)}";

    }
}
