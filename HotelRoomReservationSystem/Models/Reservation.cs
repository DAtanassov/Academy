namespace HotelRoomReservationSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public int RoomTypeId { get; set; }
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

    }
}
