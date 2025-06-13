
using System.Text.Json;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class RoomDB : ReadWriteDB, IDatabase
    {
        protected readonly static string _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data") + Path.DirectorySeparatorChar;
        protected readonly static string _path = "Rooms.json";

        public List<T> GetList<T>()
        {
            string fileContent = GetFileContent(_databasePath, _path);

            List<T>? list = new List<T>();

            if (!String.IsNullOrEmpty(fileContent))
                list = JsonSerializer.Deserialize<List<T>>(fileContent);

            if (list == null)
                return new List<T>();

            return list;
        }
        public T GetById<T>(int id)
        {
            List<T> list = GetList<T>();
            return list.FirstOrDefault(x => (x as Room).Id == id) ?? throw new KeyNotFoundException($"Room type with ID {id} not found.");

        }
        public void Insert<T>(T item)
        {
            List<T> list = GetList<T>();

            (item as Room).Id = GetMaxId<T>(list);
            list.Add(item);

            WriteToFile<T>(list);

        }
        public void Update<T>(T item)
        {
            List<T> list = GetList<T>();

            int index = list.FindIndex(x => (x as Room).Id == (item as Room).Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list);

        }
        public void Delete<T>(T item)
        {
            int roomId = (item as Room).Id;

            List<T> list = GetList<T>();

            int index = list.FindIndex(x => (x as Room).Id == roomId);
            if (index == -1)
                return;

            new ReservationDB().DeleteByRoomId(roomId);

            list.RemoveAt(index);

            WriteToFile(list);
        }

        public void DeleteByHotelId(int hotelId)
        {
            List<Room> list = GetList<Room>();
            list.RemoveAll(x => x.HotelId == hotelId);
            WriteToFile(list);
        }
        public void DeleteByRoomTypeId(int roomTypeId)
        {
            List<Room> list = GetList<Room>();
            list.RemoveAll(x => x.RoomTypeId == roomTypeId);
            WriteToFile(list);
        }

        private void WriteToFile<T>(List<T> list)
            => WriteToFile(JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }), _databasePath, _path);

        private int GetMaxId<T>(List<T> list)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => (x as Room).Id);
            return ++maxId;
        }

        public override string GetFileContent(string databasePath, string path) => base.GetFileContent(databasePath, path);
        public override void WriteToFile(string content, string databasePath, string path) => base.WriteToFile(content, databasePath, path);
    }
}
