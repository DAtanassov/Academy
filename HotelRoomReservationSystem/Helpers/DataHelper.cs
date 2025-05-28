using System.Collections.Generic;
using System.Text.Json;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class DataHelper : IDataHelper
    {
        public static void CreateDataBase()
        {

            string dbDirPath = AppDomain.CurrentDomain.BaseDirectory + "/Data/";
            if (!Directory.Exists(dbDirPath))
            {
                try
                { Directory.CreateDirectory(dbDirPath); }
                catch (Exception)
                { throw new ApplicationException("Can't create directory database."); }
            }
            
            string[] dbFilePaths = { "Users", "Hotels", "RoomTypes", "Rooms", "Reservations"};
            foreach (string dbFilePath in dbFilePaths)
            {
                string dbPath = dbDirPath + dbFilePath + ".json";
                if (!File.Exists(dbPath))
                {
                    try
                    {
                        FileStream file = File.Create(dbPath); 
                        file.Close();
                    }
                    catch (Exception)
                    { throw new ApplicationException($"Can't create database {dbFilePaths}"); }
                }
            }

        }

        public static List<User> GetUserList()
        {
            string fileContent = GetFileContent("Users.json");

            List<User>? users = new List<User>();

            if (!String.IsNullOrEmpty(fileContent))
                users = JsonSerializer.Deserialize<List<User>>(fileContent);

            if (users == null)
                return new List<User>();

            return users;
        }

        public static List<Hotel> GetHotelList()
        {
            string fileContent = GetFileContent("Hotels.json");

            List<Hotel>? hotels = new List<Hotel>();

            if (!String.IsNullOrEmpty(fileContent))
                hotels = JsonSerializer.Deserialize<List<Hotel>>(fileContent);

            if (hotels == null)
                return new List<Hotel>();

            return hotels;
        }

        public static List<RoomType> GetRoomTypeList()
        {
            string fileContent = GetFileContent("RoomTypes.json");

            List<RoomType>? roomTypes = new List<RoomType>();

            if (!String.IsNullOrEmpty(fileContent))
                roomTypes = JsonSerializer.Deserialize<List<RoomType>>(fileContent);

            if (roomTypes == null)
                return new List<RoomType>();

            return roomTypes;
        }

        public static List<Room> GetRoomList()
        {
            string fileContent = GetFileContent("Rooms.json");

            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms == null)
                return new List<Room>();

            return rooms;
        }

        public static List<Reservation> GetReservationList()
        {
            string fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations == null)
                return new List<Reservation>();

            return reservations;
        }


        public static void InsertUsers(List<User> users)
        {
            List<User> allUsers = GetUserList();
            foreach (User user in users)
            {
                user.Id = GetMaxId(allUsers);
                allUsers.Add(user);
            }
            UpdateUsers(allUsers);
        }

        public static void InsertHotels(List<Hotel> hotels)
        {
            List<Hotel> allHotels = GetHotelList();
            foreach (Hotel hotel in hotels)
            {
                hotel.Id = GetMaxId(allHotels);
                allHotels.Add(hotel);
            }
            UpdateHotels(allHotels);
        }

        public static void InsertHotelRoomTypes(List<RoomType> roomTypes)
        {
            List<RoomType> allRoomTypes = GetRoomTypeList();
            foreach (RoomType roomType in roomTypes)
            {
                roomType.Id = GetMaxId(allRoomTypes, roomType.HotelId);
                allRoomTypes.Add(roomType);
            }
            UpdateHotelRoomTypes(allRoomTypes);
            
        }

        public static void InsertHotelRooms(List<Room> rooms)
        {
            List<Room> allRooms = GetRoomList();
            foreach (Room room in rooms)
            {
                room.Id = GetMaxId(allRooms, room.HotelId);
                allRooms.Add(room);
            }
            UpdateHotelRooms(allRooms);
        }

        public static void InsertReservations(List<Reservation> reservations)
        {
            List<Reservation> allReservation = GetReservationList();
            foreach (Reservation reservation in reservations)
            {
                reservation.Id = GetMaxId(allReservation, reservation.HotelId);
                allReservation.Add(reservation);
            }
            UpdateReservations(allReservation);
        }


        public static void UpdateUsers(List<User> users)
        {
            string fileContent = JsonSerializer.Serialize(users);
            WriteUpdateFile("Users.json", fileContent);

        }

        public static void UpdateHotels(List<Hotel> hotels)
        {
            string fileContent = JsonSerializer.Serialize(hotels);
            WriteUpdateFile("Hotels.json", fileContent);

        }

        public static void UpdateHotelRoomTypes(List<RoomType> roomTypes)
        {
            string fileContent = JsonSerializer.Serialize(roomTypes);
            WriteUpdateFile("RoomTypes.json", fileContent);
        }
        
        public static void UpdateHotelRooms(List<Room> rooms)
        {
            string fileContent = JsonSerializer.Serialize(rooms);
            WriteUpdateFile("Rooms.json", fileContent);
        }

        public static void UpdateReservations(List<Reservation> reservations)
        {
            string fileContent = JsonSerializer.Serialize(reservations);
            WriteUpdateFile("Reservations.json", fileContent);

        }


        public static void DeleteHotelData(int hotelId)
        {
            string fileContent = GetFileContent("Rooms.json");

            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms != null && rooms.Count > 0)
            {
                rooms = rooms.Where(x => (x.HotelId != hotelId)).ToList();
                UpdateHotelRooms(rooms);
            }

            fileContent = GetFileContent("RoomTypes.json");

            List<RoomType>? roomTypes = new List<RoomType>();

            if (!String.IsNullOrEmpty(fileContent))
                roomTypes = JsonSerializer.Deserialize<List<RoomType>>(fileContent);

            if (roomTypes != null && roomTypes.Count > 0)
            {
                roomTypes = roomTypes.Where(x => (x.HotelId != hotelId)).ToList();
                UpdateHotelRoomTypes(roomTypes);
            }


            fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => (x.HotelId != hotelId)).ToList();
                UpdateReservations(reservations);
            }

        }

        public static void DeleteRoomData(int roomId, int hotelId)
        {
            string fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => !(x.RoomId == roomId && x.HotelId == hotelId)).ToList();
                UpdateReservations(reservations);
            }
        }

        public static void DeleteRoomTypeData(int roomTypeId, int hotelId)
        {

            string fileContent = GetFileContent("Rooms.json");

            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms != null && rooms.Count > 0)
            {
                rooms = rooms.Where(x => !(x.RoomTypeId == roomTypeId && x.HotelId == hotelId)).ToList();
                UpdateHotelRooms(rooms);
            }

            fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => !(x.RoomTypeId == roomTypeId && x.HotelId == hotelId)).ToList();
                UpdateReservations(reservations);
            }
        }


        private static int GetMaxId(List<User> list)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => x.Id);
            return ++maxId;
        }

        private static int GetMaxId(List<Hotel> list)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => x.Id);
            return ++maxId;
        }

        private static int GetMaxId(List<RoomType> list, int hotelId)
        {
            if (list.Count == 0)
                return 1;

            list = list.Where(r => r.HotelId == hotelId).ToList();
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => x.Id);
            return ++maxId;
        }

        private static int GetMaxId(List<Room> list, int hotelId)
        {
            if (list.Count == 0)
                return 1;

            list = list.Where(r => r.HotelId == hotelId).ToList();
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => x.Id);
            return ++maxId;
        }

        private static int GetMaxId(List<Reservation> list, int hotelId)
        {
            if (list.Count == 0)
                return 1;

            list = list.Where(r => r.HotelId == hotelId).ToList();
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => x.Id);
            return ++maxId;
        }


        public static string GetFileContent(string path)
        {
            string dbDirPath = AppDomain.CurrentDomain.BaseDirectory + "/Data/";
            string dbPath = dbDirPath + path;

            if (!Directory.Exists(dbDirPath))
            {
                try
                { Directory.CreateDirectory(dbDirPath); }
                catch (Exception)
                { throw new ApplicationException("Can't create directory database."); }
            }

            if (!File.Exists(dbPath))
            {
                try
                {
                    FileStream file = File.Create(dbPath);
                    file.Close();
                }
                catch (Exception)
                { throw new ApplicationException("Can't write to database"); }
            }

            string fileContent = "";
            try
            {
                fileContent = File.ReadAllText(dbPath);
            }
            catch (Exception)
            { throw new ApplicationException("Can't read database"); }

            return fileContent;
        }

        public static void WriteUpdateFile(string path, string fileContent)
        {
            string dbDirPath = AppDomain.CurrentDomain.BaseDirectory + "/Data/";
            string dbPath = dbDirPath + path;

            if (!Directory.Exists(dbDirPath))
            {
                try
                { Directory.CreateDirectory(dbDirPath); }
                catch (Exception)
                { throw new ApplicationException("Can't create directory database."); }
            }

            try
            { File.WriteAllText(dbPath, fileContent); }
            catch (Exception)
            { throw new ApplicationException("Can't write to database"); }
        }

    }
}
