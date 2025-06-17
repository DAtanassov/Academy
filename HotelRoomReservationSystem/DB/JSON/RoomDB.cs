
using HotelRoomReservationSystem.DB.Interfaces;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class RoomDB : ReadWriteDB<Room>, IDatabase<Room>
    {
        protected readonly static string _path = "Rooms.json";

        public List<Room> GetList() => base.GetAllItemsFromFile(_path);

        public Room GetById(int id)
        {
            List<Room> list = GetList();
            return list.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException($"Room type with ID {id} not found.");

        }
        public void Insert(Room item)
        {
            List<Room> list = GetList();

            item.Id = list.Any() ? list.Max(i => i.Id) + 1 : 1;
            list.Add(item);

            WriteToFile(list, _path);

        }
        public void Update(Room item)
        {
            List<Room> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list, _path);

        }
        public void Delete(Room item)
        {
            List<Room> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            new ReservationDB().DeleteByRoomId(item.Id);

            list.RemoveAt(index);

            WriteToFile(list, _path);
        }

        public void DeleteByHotelId(int hotelId)
        {
            List<Room> list = GetList();
            list.RemoveAll(x => x.HotelId == hotelId);
            WriteToFile(list, _path);
        }
        public void DeleteByRoomTypeId(int roomTypeId)
        {
            List<Room> list = GetList();
            list.RemoveAll(x => x.RoomTypeId == roomTypeId);
            WriteToFile(list, _path);
        }

        public override void WriteToFile(List<Room> items, string path) => base.WriteToFile(items, path);
    }
}
