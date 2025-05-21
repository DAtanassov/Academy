using System.Text.Json;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class RoomHelper
    {
        private static DataHelper dataHelper = new DataHelper();

        public List<Room> GetRooms(int[]? hotelId)
        {
            string fileContent = dataHelper.GetFileContent("Rooms.json");
            
            List<Room>? rooms = new List<Room>();

            if (!String.IsNullOrEmpty(fileContent))
                rooms = JsonSerializer.Deserialize<List<Room>>(fileContent);

            if (rooms == null)
                return new List<Room>();

            if (rooms.Count > 0 && hotelId != null)
                rooms = rooms.Where(x => hotelId.Contains(x.HotelId)).ToList();

            return rooms;
        }

        public List<RoomType> GetRoomTypes(int[]? hotelId = null)
        {
            string fileContent = dataHelper.GetFileContent("RoomTypes.json");

            List<RoomType>? roomTypes = new List<RoomType>();

            if (!String.IsNullOrEmpty(fileContent))
                roomTypes = JsonSerializer.Deserialize<List<RoomType>>(fileContent);

            if (roomTypes == null)
                return new List<RoomType>();

            if (roomTypes.Count > 0 && hotelId != null)
                roomTypes = roomTypes.Where(x => (hotelId.Contains(x.HotelId))).ToList();

            return roomTypes;
        }

        public void ShowRooms(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tRoom in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms([hotel.Id]);

            int counter = 0;
            foreach (Room r in rooms)
                Console.WriteLine($"\t{++counter}. {r.RoomPresentation()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public void ShowRoomTypes(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tRoom types in hotel \"{hotel.Name}\"\n");

            List<RoomType> roomTypes = GetRoomTypes([hotel.Id]);

            int counter = 0;
            foreach (RoomType rt in roomTypes)
                Console.WriteLine($"\t{++counter}. {rt.Name}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public Room? AddRoom(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tAdding room in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms(null);

            Console.Write("\tRoom number: ");
            int number;
            if (!int.TryParse(Console.ReadLine(), out number))
            {
                Console.WriteLine("Number cannot be empty!\nRoom not created.");
                return null;
            }

            RoomType? roomType = SelectRoomType(hotel);

            if (roomType == null)
            {
                Console.WriteLine("Room type cannot be empty!\nRoom not created.\n" +
                    "Press any key to continue...");
                Console.ReadKey();
                return null;
            }

            Room room = new Room(number, roomType.Id);
            room.Id = GetMaxRoomId(rooms);
            room.HotelId = hotel.Id;

            Console.Clear();
            Console.Write("\tEnter price per night: ");
            string inputText = Console.ReadLine() ?? "0.00";
            decimal pricePerNight;
            if (!decimal.TryParse(inputText, out pricePerNight))
                if ((inputText.Contains('.') && decimal.TryParse(inputText.Replace('.', ','), out pricePerNight))
                    || (inputText.Contains(',') && decimal.TryParse(inputText.Replace(',', '.'), out pricePerNight)))
                    room.PricePerNight = pricePerNight;

            
            Console.Clear();
            Console.Write("\tEnter cancellation fee: ");
            inputText = Console.ReadLine() ?? "0.00";
            decimal cancellationFee;
            if (!decimal.TryParse(inputText, out cancellationFee))
                if ((inputText.Contains('.') && decimal.TryParse(inputText.Replace('.', ','), out cancellationFee))
                    || (inputText.Contains(',') && decimal.TryParse(inputText.Replace(',', '.'), out cancellationFee)))
                        room.CancellationFee = cancellationFee;
                


            rooms.Add(room);
            dataHelper.WriteUpdateHotelRooms(hotel, rooms);
            
            return room;
        }

        public RoomType? AddRoomType(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tAdding room type in hotel \"{hotel.Name}\"\n");

            List<RoomType> roomTypes = GetRoomTypes(null);

            Console.Write("\tRoom type name: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name cannot be empty!\nRoom type not created.");
                return null;
            }

            List<string> amenities = new List<string>();
            Console.Clear();
            Console.Write($"\tAdd room type amenities? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                string amenitiesName;
                while (true)
                {
                    Console.Clear();
                    Console.Write("\t");
                    amenitiesName = Console.ReadLine() ?? string.Empty;
                    if (!string.IsNullOrEmpty(amenitiesName))
                        amenities.Add(amenitiesName);

                    Console.Clear();
                    Console.Write($"\tAdd more? (\"Y/n\"): ");
                    if ((Console.ReadLine() ?? "n").ToLower() != "y")
                        break;
                }
            }

            Console.Clear();
            Console.Write("\tEnter maximum occupancy: ");
            int maximumOccupancy;
            if (!int.TryParse(Console.ReadLine() ?? "0", out maximumOccupancy))
                maximumOccupancy = 0;

            RoomType roomType = new RoomType(name, hotel.Id, amenities, maximumOccupancy);
            roomType.Id = GetMaxRoomTypesId(roomTypes);

            roomTypes.Add(roomType);
            dataHelper.WriteUpdateHotelRoomTypes(hotel, roomTypes);

            return roomType;
        }

        public Room? SelectRoom(Hotel hotel)
        {
            List<Room> rooms = GetRooms([hotel.Id]);

            if (rooms.Count == 0)
                return null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t\tRooms in hotel {hotel.Name}\n");

                int counter = 0;
                foreach (Room r in rooms)
                    Console.WriteLine($"\t{++counter}. {r.RoomPresentation()}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a room: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= rooms.Count)
                        return rooms[choice - 1];
                    else if (choice == counter)
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                    }
                }
            }

            return null;
        }

        public RoomType? SelectRoomType(Hotel hotel)
        {
            List<RoomType> roomTypes = GetRoomTypes([hotel.Id]);

            if (roomTypes.Count == 0)
                return null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t\tRoom types in hotel {hotel.Name}\n");

                int counter = 0;
                foreach (RoomType rt in roomTypes)
                    Console.WriteLine($"\t{++counter}. {rt.Name}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a room type: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= roomTypes.Count)
                        return roomTypes[choice - 1];
                    else if (choice == counter)
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                    }
                }
            }

            return null;
        }

        public RoomType? GetRoomTypeById(int id, int hotelId)
        {

            List<RoomType> roomTypes = GetRoomTypes([hotelId]);
            roomTypes = roomTypes.Where(x => (x.Id == id)).ToList();

            if (roomTypes.Count > 0)
                return roomTypes[0];

            return null;
        }

        // TODO - Edit room
        // TODO - Delete room

        // TODO - Edit room type
        // TODO - Delete room type

        public int GetMaxRoomId(List<Room> rooms)
        {
            if (rooms.Count == 0)
                return 1;

            int maxId = rooms.Max(x => x.Id);
            return ++maxId;
        }
        
        public int GetMaxRoomTypesId(List<RoomType> roomTypes)
        {
            if (roomTypes.Count == 0)
                return 1;

            int maxId = roomTypes.Max(x => x.Id);
            return ++maxId;
        }
    }
}
