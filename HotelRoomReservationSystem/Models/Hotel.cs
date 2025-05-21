using HotelRoomReservationSystem.Helpers;

namespace HotelRoomReservationSystem.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        
        public Hotel() { }
        public Hotel(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }

        public string HotelInfo()
        {
            // TODO - Room count, available...
            return $"Hotel \"{Name}\"\n" +
                $"Adress: \"{Address}\"";
        }

    }
}
