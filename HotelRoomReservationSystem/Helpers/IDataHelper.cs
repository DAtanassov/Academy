using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public interface IDataHelper
    {
        abstract static void CreateDataBase();

        abstract static List<User> GetUserList();
        abstract static List<Hotel> GetHotelList();
        abstract static List<RoomType> GetRoomTypeList();
        abstract static List<Room> GetRoomList();
        abstract static List<Reservation> GetReservationList();

        abstract static void InsertUsers(List<User> users);
        abstract static void InsertHotels(List<Hotel> hotels);
        abstract static void InsertHotelRoomTypes(List<RoomType> roomTypes);
        abstract static void InsertHotelRooms(List<Room> rooms);
        abstract static void InsertReservations(List<Reservation> reservations);

        abstract static void UpdateUsers(List<User> users);
        abstract static void UpdateHotels(List<Hotel> hotels);
        abstract static void UpdateHotelRoomTypes(List<RoomType> roomTypes);
        abstract static void UpdateHotelRooms(List<Room> rooms);
        abstract static void UpdateReservations(List<Reservation> reservations);

        abstract static void DeleteHotelData(int hotelId);
        abstract static void DeleteRoomData(int roomId, int hotelId);
        abstract static void DeleteRoomTypeData(int roomTypeId, int hotelId);

    }
}
