namespace HotelRoomReservationSystem.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HotelId { get; set; }
        public List<string> Amenities { get; set; }
        public int MaximumOccupancy {  get; set; }
        
        public RoomType() { }
        public RoomType(string name, int hotelId)
        {
            this.HotelId = hotelId;
            this.Name = name;
            this.Amenities = new List<string>();
            this.MaximumOccupancy = 0;
        }
        public RoomType(string name, int hotelId, List<string> amenities, int maximumOccupancy)
        {
            this.HotelId = hotelId; 
            this.Name = name;
            this.Amenities = amenities;
            this.MaximumOccupancy = maximumOccupancy;
        }

    }
}
