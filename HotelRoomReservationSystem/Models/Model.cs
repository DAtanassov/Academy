namespace HotelRoomReservationSystem.Models
{
    public abstract class Model
    {
        public int Id { get; set; }

        public abstract string Info();
        public abstract string ShortInfo();
    }
}
