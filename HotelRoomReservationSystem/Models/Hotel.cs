using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public class Hotel : BaseModel
    {
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
            List<Room> rooms = RoomHelper.GetRooms([Id]);
            List<Reservation> reservations = ReservationHelper.GetHotelReservations([Id]);
            int allRooms = rooms.Count;
            int notAvalaible = reservations.Where(r => (r.Status == RoomStatus.booked || r.Status == RoomStatus.ocupated)
                                                        && (r.CheckInDate <= DateTime.Now.Date && DateTime.Now.Date <= r.CheckOutDate)).Count();
            User? manager = new UserHelper().GetUserById(ManagerId);

            return $"Hotel \"{Name}\" ({Address})" +
                ((manager == null) ? "" : $", manager {manager.Name}, ") +
                $" all rooms {allRooms}, available {allRooms - notAvalaible}";
        }

        public override string ShortInfo()
        {
            return $"Hotel \"{Name}\"" + ((String.IsNullOrWhiteSpace(Address)) ? "" : $" ({Address})");
        }

    }
}
