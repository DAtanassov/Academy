
using HotelRoomReservationSystem.DB.Interfaces;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.DB.JSON
{
    public class ReservationDB : ReadWriteDB<Reservation>, IDatabase<Reservation>
    {
        protected readonly static string _path = "Reservations.json";

        public List<Reservation> GetList() => base.GetAllItemsFromFile(_path);

        public Reservation GetById(int id)
        {
            List<Reservation> list = GetList();
            return list.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException($"Reservation type with ID {id} not found.");

        }
        public void Insert(Reservation item)
        {
            List<Reservation> list = GetList();

            item.Id = list.Any() ? list.Max(i => i.Id) + 1 : 1;
            list.Add(item);

            WriteToFile(list, _path);

        }
        public void Update(Reservation item)
        {
            List<Reservation> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list[index] = item;

            WriteToFile(list, _path);

        }
        public void Delete(Reservation item)
        {

            List<Reservation> list = GetList();

            int index = list.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                return;

            list.RemoveAt(index);

            WriteToFile(list, _path);
        }

        public void DeleteByHotelId(int hotelId)
        {
            List<Reservation> list = GetList();
            list.RemoveAll(x => x.HotelId == hotelId);
            WriteToFile(list, _path);
        }

        public void DeleteByRoomId(int roomId)
        {
            List<Reservation> list = GetList();
            list.RemoveAll(x => x.RoomId == roomId);
            WriteToFile(list, _path);
        }

        public void DeleteByRoomTypeId(int roomTypeId)
        {
            List<Reservation> list = GetList();
            list.RemoveAll(x => x.RoomTypeId == roomTypeId);
            WriteToFile(list, _path);
        }

        public override void WriteToFile(List<Reservation> items, string path) => base.WriteToFile(items, path);
    }
}
