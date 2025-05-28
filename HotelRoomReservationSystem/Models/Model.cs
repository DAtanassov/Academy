namespace HotelRoomReservationSystem.Models
{
    public abstract class Model
    {
        public int Id { get; set; }

        public abstract string Info();
        public abstract string ShortInfo();
        public virtual string GetId() => Id.ToString();
    }
}
