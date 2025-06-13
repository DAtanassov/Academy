namespace HotelRoomReservationSystem.Models
{
    public interface IModel
    {
        int Id { get; set; }

        string Info();
        string ShortInfo();
    }
}
