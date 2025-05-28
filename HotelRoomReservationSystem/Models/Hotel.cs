using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public class Hotel : Model
    {
        //public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public int ManagerId { get; set; }

        public Hotel() { }

        public Hotel(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        public override string Info()
        {
            // TODO - more info...
            List<Room> rooms = RoomHelper.GetRooms([Id]);
            List<Reservation> reservations = ReservationHelper.GetHotelReservations([Id]);
            int allRooms = rooms.Count;
            int notAvalaible = reservations.Where(r => (r.Status == RoomStatus.booked || r.Status == RoomStatus.ocupated)
                                                        && (r.CheckInDate <= DateTime.Now.Date && DateTime.Now.Date <= r.CheckOutDate)).Count();
            return $"Hotel \"{Name}\" ({Address})" +
                   $" all rooms {allRooms}, available {allRooms - notAvalaible}";
        }

        public override string ShortInfo()
        {
            return $"Hotel \"{Name}\"" + ((String.IsNullOrWhiteSpace(Address)) ? "" : $" ({Address})");
        }

    }
}
