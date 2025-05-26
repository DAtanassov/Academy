using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public int ManagerId { get; set; }

        public Hotel() { }
        public Hotel(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        public string ShortInfo()
        {
            return $"Hotel \"{Name}\"" + ((String.IsNullOrWhiteSpace(Address)) ? "" : $" ({Address})");
        }

        public string Info()
        {
            // TODO - more info...
            List<Room> rooms = (new HotelHelper()).GetRooms([Id]);
            List<Reservation> reservations = (new ReservationHelper()).GetHotelReservations([Id]);
            int allRooms = rooms.Count;
            int notAvalaible = reservations.Where(r => (r.Status == RoomStatus.booked || r.Status == RoomStatus.ocupated)
                                                        && (r.CheckInDate <= DateTime.Now.Date && DateTime.Now.Date <= r.CheckOutDate)).Count();
            return $"Hotel \"{Name}\" ({Address})" +
                   $" all rooms {allRooms}, available {allRooms - notAvalaible}";
        }

    }
}
