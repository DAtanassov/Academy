namespace HotelRoomReservationSystem.Models
{
    public class RoomType : HotelModel
    {
        //public int Id { get; set; }
        public string Name { get; set; } = "";
        //public int HotelId { get; set; }
        public List<string> Amenities { get; set; } = new List<string>();
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

        public override string Info()
        {
            return Name;
        }

        public override string ShortInfo()
        {
            return Name;
        }
    }
}
