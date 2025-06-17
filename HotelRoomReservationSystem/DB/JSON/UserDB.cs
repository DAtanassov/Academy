using HotelRoomReservationSystem.DB.Interfaces;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class UserDB : ReadWriteDB<User>, IDatabase<User>
    {
        protected readonly static string _path = "Users.json";

        public List<User> GetList() => base.GetAllItemsFromFile(_path);
        public User GetById(int id)
        {
            List<User> list = GetList();
            return list.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        }
        public void Insert(User item)
        {
            List<User> list = GetList();

            item.Id = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            list.Add(item);

            WriteToFile(list, _path);

        }
        public void Update(User item)
        {
            List<User> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list, _path);

        }
        public void Delete(User item)
        {
            List<User> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list.RemoveAt(index);

            WriteToFile(list, _path);
        }

        public override void WriteToFile(List<User> items, string path) => base.WriteToFile(items, path);
    }
}
