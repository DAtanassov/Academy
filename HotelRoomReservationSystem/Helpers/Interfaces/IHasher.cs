namespace HotelRoomReservationSystem.Helpers.Interfaces
{
    public interface IHasher
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
