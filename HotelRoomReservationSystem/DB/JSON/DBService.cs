
using HotelRoomReservationSystem.DB.Interfaces;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class DBService<T> : IDatabase<T>
    {
        
        IDatabase<T> _objectDB;
        
        public DBService(IDatabase<T> objectDB)
        {
            _objectDB = objectDB ?? throw new ArgumentNullException(nameof(objectDB), "Database cannot be null");
        }

        public List<T> GetList()
        {
            return _objectDB.GetList();
        }
        public T GetById(int id)
        {
            return _objectDB.GetById(id);
        }
        public void Insert(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item cannot be null");
            _objectDB.Insert(item);
        }
        public void Update(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item cannot be null");
            _objectDB.Update(item);
        }
        public void Delete(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item cannot be null");
            _objectDB.Delete(item);
        }

    }
}
