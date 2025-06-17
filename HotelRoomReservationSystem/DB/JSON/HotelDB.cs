
using HotelRoomReservationSystem.DB.Interfaces;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class HotelDB : ReadWriteDB<Hotel>, IDatabase<Hotel>
    {
        protected readonly static string _path = "Hotels.json";

        public List<Hotel> GetList() => base.GetAllItemsFromFile(_path);
        
        public Hotel GetById(int id)
        {
            List<Hotel> list = GetList();
            return list.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException($"Hotel with ID {id} not found.");
        }
        public void Insert(Hotel item)
        {
            List<Hotel> list = GetList();

            item.Id = list.Any() ? list.Max(i => i.Id) + 1 : 1;
            list.Add(item);

            WriteToFile(list, _path);
        }
        public void Update(Hotel item)
        {
            List<Hotel> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list, _path);
        }
        public void Delete(Hotel item)
        {
            List<Hotel> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            new RoomDB().DeleteByHotelId(item.Id);
            new RoomTypeDB().DeleteByHotelId(item.Id);
            new ReservationDB().DeleteByHotelId(item.Id);

            list.RemoveAt(index);

            WriteToFile(list, _path);
        }

        public override void WriteToFile(List<Hotel> items, string path) => base.WriteToFile(items, path);
    }

}
