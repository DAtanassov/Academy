using HotelRoomReservationSystem.DB.Interfaces;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class RoomTypeDB : ReadWriteDB<RoomType>, IDatabase<RoomType>
    {
        protected readonly static string _path = "RoomTypes.json";

        public List<RoomType> GetList() => base.GetAllItemsFromFile(_path);
        public RoomType GetById(int id)
        {
            List<RoomType> list = GetList();
            return list.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException($"Room type with ID {id} not found.");

        }
        public void Insert(RoomType item)
        {
            List<RoomType> list = GetList();

            item.Id = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            list.Add(item);

            WriteToFile(list, _path);

        }
        public void Update(RoomType item)
        {
            List<RoomType> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list, _path);

        }
        public void Delete(RoomType item)
        {
            List<RoomType> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            new RoomDB().DeleteByRoomTypeId(item.Id);
            new ReservationDB().DeleteByRoomTypeId(item.Id);

            list.RemoveAt(index);

            WriteToFile(list, _path);
        }

        public void DeleteByHotelId(int hotelId)
        {
            List<RoomType> list = GetList();
            list.RemoveAll(x => x.HotelId == hotelId);
            WriteToFile(list, _path);
        }

        public override void WriteToFile(List<RoomType> items, string path) => base.WriteToFile(items, path);
    }
}
