using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public enum RoomStatus
    {
        available = 0,
        booked = 1,
        ocupated = 2,
        canceledByUser = 3,
        canceledByHotel = 4,
        expired = 5,
        completed = 6
    }

    public class Room
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal CancellationFee { get; set; }
        public int HotelId { get; set; }

        public Room() { }

        public Room(int number, int roomTypeId, int hotelId)
        {
            this.Number = number;
            this.RoomTypeId = roomTypeId;
            this.HotelId = hotelId;
            this.PricePerNight = 0m;
            this.CancellationFee = 0m;
        }
        public Room(int number, int roomTypeId, int hotelId, decimal pricePerNight, decimal cancellationFee)
        {
            this.Number = number;
            this.RoomTypeId = roomTypeId;
            this.HotelId = hotelId;
            this.PricePerNight = pricePerNight;
            this.CancellationFee = cancellationFee;
        }

        public string Presentation(bool showStatus = false)
        {
            RoomType? roomType = GetRoomTypeById();

            if (roomType == null)
                return $"{Number}";

            return $"{roomType.Name} {Number}";
        }

        // TODO - RoomPresentation with status 

        public string RoomInfo() 
        {
            // TODO - add more room type and Hotel info

            RoomType? roomType = GetRoomTypeById();

            return $"\t\tNumber: {Number},\n" +
                    ((roomType == null) ? "" : $"\t\ttype: {roomType.Name},\n") +
                    $"\t\tprice per night: {PricePerNight},\n" +
                    $"\t\tcancellation fee: {CancellationFee},\n";
                    //$"\t\tstatus {status}";
        }

        private RoomType? GetRoomTypeById()
        {
            return new HotelHelper().GetRoomTypeById(RoomTypeId, HotelId);
        }
        public string RoomID()
        {
            return $"{Id}-{HotelId}";
        }
    }
}
