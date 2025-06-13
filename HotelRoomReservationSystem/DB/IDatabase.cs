
namespace HotelRoomReservationSystem.DB
{
    public interface IDatabase
    {
        List<T> GetList<T>();
        T GetById<T>(int id);
        void Insert<T>(T item);
        void Update<T>(T item);
        void Delete<T>(T item);
    }
}
