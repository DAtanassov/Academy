using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public class Room : BaseModel
    {
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public int Number { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal CancellationFee { get; set; }

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

        public override string ShortInfo()
        {
            RoomType? roomType = RoomTypeHelper.GetRoomTypeById(RoomTypeId, HotelId);

            if (roomType == null)
                return $"{Number}";

            return $"{roomType.Name} {Number}";
        }

        public override string Info() 
        {
            RoomType? roomType = RoomTypeHelper.GetRoomTypeById(RoomTypeId, HotelId);

            return $"\t\tNumber: {Number},\n" +
                    ((roomType == null) ? "" : $"\t\ttype: {roomType.Name},\n") +
                    $"\t\tprice per night: {PricePerNight},\n" +
                    $"\t\tcancellation fee: {CancellationFee},\n";
        }
    }
}
