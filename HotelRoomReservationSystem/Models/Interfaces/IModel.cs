namespace HotelRoomReservationSystem.Models.Interfaces
{
    public interface IModel
    {
        int Id { get; set; }

        string Info();
        string ShortInfo();
    }
}
