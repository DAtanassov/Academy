
namespace HotelRoomReservationSystem.DB.JSON
{
    public class DBService
    {
        
        IDatabase _objectDB;
        
        public DBService(IDatabase objectDB)
        {
            _objectDB = objectDB ?? throw new ArgumentNullException(nameof(objectDB), "Database cannot be null");
        }

        public List<T> GetList<T>()
        {
            return _objectDB.GetList<T>();
        }
        public T GetById<T>(int id)
        {
            return _objectDB.GetById<T>(id);
        }
        public void Insert<T>(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item cannot be null");
            _objectDB.Insert(item);
        }
        public void Update<T>(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item cannot be null");
            _objectDB.Update(item);
        }
        public void Delete<T>(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item cannot be null");
            _objectDB.Delete(item);
        }

        public void ClearDatabase<T>()
        {
            var items = _objectDB.GetList<T>();
            foreach (var item in items)
            {
                _objectDB.Delete(item);
            }
        }
        public void ClearDatabase()
        {
            var allTypes = _objectDB.GetList<object>();
            foreach (var item in allTypes)
            {
                _objectDB.Delete(item);
            }
        }

    }
}
