using System.Data.Common;
using System.Text.Json;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class DataHelper : IDataHelper
    {
        protected readonly static string dbDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data") + Path.DirectorySeparatorChar;

        public static void CreateDataBase()
        {
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
                    { throw new ApplicationException($"Can't create file {dbFilePath + ".json"}"); }
                }
            }

        }

        public static List<T> GetList<T>(string dbPath)
        {
            string fileContent = GetFileContent(dbPath);

            List<T>? list = new List<T>();

            if (!String.IsNullOrEmpty(fileContent))
                list = JsonSerializer.Deserialize<List<T>>(fileContent);

            if (list == null)
                return new List<T>();

            return list;
        }

        public static void Insert<T>(List<T> list, string dbPath)
        {
            List<T> allT = GetList<T>(dbPath);
            foreach (T item in list)
            {
                (item as BaseModel).Id = GetMaxId(allT);
                    allT.Add(item);
            }
            Update<T>(allT, dbPath);
        }

        public static void Update<T>(List<T> list, string dbPath)
        {
            string fileContent = JsonSerializer.Serialize(list);
            WriteUpdateFile(dbPath, fileContent);

        }

        public static void Delete<T>(List<T> list, string dbPath)
        {
            List<T> allT = GetList<T>(dbPath);

            foreach (T item in list)
            {
                int index = allT.FindIndex(u => (u as BaseModel).Id == (item as BaseModel).Id);
                if (index == -1)
                    continue;

                allT.RemoveAt(index);
            }
            Update(allT, dbPath);
        }

        public static List<User> GetUserList()
            => GetList<User>("Users.json");

        public static List<Hotel> GetHotelList()
            => GetList<Hotel>("Hotels.json");

        public static List<RoomType> GetRoomTypeList()
            => GetList<RoomType>("RoomTypes.json");

        public static List<Room> GetRoomList()
            => GetList<Room>("Rooms.json");

        public static List<Reservation> GetReservationList()
            => GetList<Reservation>("Reservations.json");

        public static void InsertUsers(List<User> users)
            => Insert(users, "Users.json");
        
        public static void InsertHotels(List<Hotel> hotels)
            => Insert(hotels, "Hotels.json");

        public static void InsertHotelRoomTypes(List<RoomType> roomTypes)
            => Insert(roomTypes, "RoomTypes.json");

        public static void InsertHotelRooms(List<Room> rooms)
            => Insert(rooms, "Rooms.json");

        public static void InsertReservations(List<Reservation> reservations)
            => Insert(reservations, "Reservations.json");
        
        public static void UpdateUsers(List<User> users)
        {
            string fileContent = JsonSerializer.Serialize(users);
            WriteUpdateFile("Users.json", fileContent);

        }

        public static void UpdateHotels(List<Hotel> hotels)
            => Update(hotels, "Hotels.json");

        public static void UpdateHotelRoomTypes(List<RoomType> roomTypes)
            => Update(roomTypes, "RoomTypes.json");
        
        public static void UpdateHotelRooms(List<Room> rooms)
            => Update(rooms, "Rooms.json");

        public static void UpdateReservations(List<Reservation> reservations)
            => Update(reservations, "Reservations.json");

        public static void DeleteHotels(List<Hotel> hotels)
            => Delete(hotels, "Hotels.json");

        public static void DeleteRooms(List<Room> rooms)
            => Delete(rooms, "Rooms.json");

        public static void DeleteRoomTypes(List<RoomType> roomTypes)
            => Delete(roomTypes, "RoomTypes.json");

        public static void DeleteReservations(List<Reservation> reservations)
            => Delete(reservations, "Reservations.json");

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

        public static void DeleteRoomData(int roomId)
        {
            string fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => !(x.RoomId == roomId)).ToList();
                UpdateReservations(reservations);
            }
        }

        public static void DeleteRoomTypeData(int roomTypeId)
        {
            string fileContent = GetFileContent("Rooms.json");

            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms != null && rooms.Count > 0)
            {
                rooms = rooms.Where(x => !(x.RoomTypeId == roomTypeId)).ToList();
                UpdateHotelRooms(rooms);
            }

            fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => !(x.RoomTypeId == roomTypeId)).ToList();
                UpdateReservations(reservations);
            }
        }

        public static int GetMaxId<T>(List<T> list)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => (x as BaseModel).Id);
            return ++maxId;
        }

        public static string GetFileContent(string path)
        {
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
