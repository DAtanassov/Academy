namespace HotelRoomReservationSystem.Helpers
{
    public interface IDataHelper
    {
        abstract static void CreateDataBase();

        abstract static List<T> GetList<T>(string dbPath);
        abstract static void Insert<T>(List<T> list, string dbPath);
        abstract static void Update<T>(List<T> list, string dbPath);
        abstract static void Delete<T>(List<T> list, string dbPath);

        abstract static void DeleteHotelData(int hotelId);
        abstract static void DeleteRoomData(int roomId);
        abstract static void DeleteRoomTypeData(int roomTypeId);

    }
}
