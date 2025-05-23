﻿using HotelRoomReservationSystem.Models;
using System.Text.Json;

namespace HotelRoomReservationSystem.Helpers
{
    public class DataHelper
    {
        public void CreateDataBaseStructure()
        {

            string dbDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\";
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

        public void DeleteHotelDataBase(int hotelId)
        {
            string fileContent = GetFileContent("Rooms.json");

            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms != null && rooms.Count > 0)
            {
                rooms = rooms.Where(x => (x.HotelId != hotelId)).ToList();
                WriteUpdateHotelRooms(rooms);
            }

            fileContent = GetFileContent("RoomTypes.json");

            List<RoomType>? roomTypes = new List<RoomType>();

            if (!String.IsNullOrEmpty(fileContent))
                roomTypes = JsonSerializer.Deserialize<List<RoomType>>(fileContent);

            if (roomTypes != null && roomTypes.Count > 0)
            {
                roomTypes = roomTypes.Where(x => (x.HotelId != hotelId)).ToList();
                WriteUpdateHotelRoomTypes(roomTypes);
            }


            fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => (x.HotelId != hotelId)).ToList();
                WriteUpdateReservations(reservations);
            }

        }

        public void DeleteRoomDataBase(int roomId, int hotelId)
        {
            string fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => !(x.RoomId == roomId && x.HotelId == hotelId)).ToList();
                WriteUpdateReservations(reservations);
            }
        }

        public void DeleteRoomTypeDataBase(int roomTypeId, int hotelId)
        {

            string fileContent = GetFileContent("Rooms.json");

            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms != null && rooms.Count > 0)
            {
                rooms = rooms.Where(x => !(x.RoomTypeId == roomTypeId && x.HotelId == hotelId)).ToList();
                WriteUpdateHotelRooms(rooms);
            }

            fileContent = GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations != null && reservations.Count > 0)
            {
                reservations = reservations.Where(x => !(x.RoomTypeId == roomTypeId && x.HotelId == hotelId)).ToList();
                WriteUpdateReservations(reservations);
            }
        }

        public string GetFileContent(string path)
        {
            string dbDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\";
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

        public void WriteUpdateUsers(List<User> users)
        {
            string fileContent = JsonSerializer.Serialize(users);
            WriteUpdateFile("Users.json", fileContent);

        }

        public void WriteUpdateHotels(List<Hotel> hotels)
        {
            string fileContent = JsonSerializer.Serialize(hotels);
            WriteUpdateFile("Hotels.json", fileContent);

        }

        public void WriteUpdateHotelRoomTypes(List<RoomType> roomTypes)
        {
            string dbPath = "RoomTypes.json";
            string fileContent = JsonSerializer.Serialize(roomTypes);

            WriteUpdateFile(dbPath, fileContent);
        }
        
        public void WriteUpdateHotelRooms(List<Room> rooms)
        {
            string dbPath = "Rooms.json";
            string fileContent = JsonSerializer.Serialize(rooms);

            WriteUpdateFile(dbPath, fileContent);
        }

        public void WriteUpdateReservations(List<Reservation> reservations)
        {
            string fileContent = JsonSerializer.Serialize(reservations);
            WriteUpdateFile("Reservations.json", fileContent);

        }

        public void WriteUpdateFile(string path, string fileContent)
        {
            string dbDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\";
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
