using System.Text.Json;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class UserDB : ReadWriteDB, IDatabase
    {
        protected readonly static string _databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data") + Path.DirectorySeparatorChar;
        protected readonly static string _path = "Users.json";

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
            return list.FirstOrDefault(x => (x as User).Id == id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        }
        public void Insert<T>(T item)
        {
            List<T> list = GetList<T>();

            (item as User).Id = GetMaxId<T>(list);
            list.Add(item);

            WriteToFile<T>(list);

        }
        public void Update<T>(T item)
        {
            List<T> list = GetList<T>();

            int index = list.FindIndex(x => (x as User).Id == (item as User).Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list);

        }
        public void Delete<T>(T item)
        {

            List<T> list = GetList<T>();

            int index = list.FindIndex(x => (x as User).Id == (item as User).Id);
            if (index == -1)
                return;

            list.RemoveAt(index);

            WriteToFile(list);
        }

        private void WriteToFile<T>(List<T> list)
            => WriteToFile(JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }), _databasePath, _path);

        private int GetMaxId<T>(List<T> list)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => (x as User).Id);
            return ++maxId;
        }

        public override string GetFileContent(string databasePath, string path) => base.GetFileContent(databasePath, path);
        public override void WriteToFile(string content, string databasePath, string path) => base.WriteToFile(content, databasePath, path);
    }
}
