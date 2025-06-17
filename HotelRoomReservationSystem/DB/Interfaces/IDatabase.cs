namespace HotelRoomReservationSystem.DB.Interfaces
{
    public interface IDatabase<T>
    {
        List<T> GetList();
        T GetById(int id);
        void Insert(T item);
        void Update(T item);
        void Delete(T item);
    }
}
