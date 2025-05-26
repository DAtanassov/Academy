using HotelRoomReservationSystem.Models;
using System.Text.Json;

namespace HotelRoomReservationSystem.Helpers
{
    public class HotelHelper
    {
        private static DataHelper dataHelper = new DataHelper();

        //################################ Hotel #####################################

        public List<Hotel> GetHotels(int[]? hotelId = null)
        {

            string fileContent = dataHelper.GetFileContent("Hotels.json");

            List<Hotel>? hotels = new List<Hotel>();

            if (!String.IsNullOrEmpty(fileContent))
                hotels = JsonSerializer.Deserialize<List<Hotel>>(fileContent);

            if (hotels == null)
                return new List<Hotel>();

            if (hotelId != null)
                hotels = hotels.Where(h => hotelId.Contains(h.Id)).ToList();

            return hotels;
        }

        public Hotel? GetHotelById(int hotelId)
        {
            List<Hotel> hotels = GetHotels([hotelId]);
            if (hotels.Count == 0)
                return null;
            return hotels[0];
        }

        public void ShowAll()
        {
            Console.Clear();
            Console.WriteLine("\t\tHotels\n");

            List<Hotel> hotels = GetHotels();

            int counter = 0;
            foreach (Hotel h in hotels)
                Console.WriteLine($"\t{++counter}. {h.ShortInfo()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public Hotel? SelectHotel(Hotel? hotel = null)
        {
            List<Hotel> hotels = GetHotels();

            if (hotels.Count == 0)
                return hotel;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\t\tHotels\n");

                int counter = 0;
                foreach (Hotel h in hotels)
                    Console.WriteLine($"\t{++counter}. {h.Name}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a hotel: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= hotels.Count)
                        return hotels[choice - 1];
                    else if (choice == counter)
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            break;

                    }
                }
            }

            return hotel;
        }

        public Hotel? AddHotel()
        {
            List<Hotel> hotels = GetHotels();

            Console.Clear();
            Console.WriteLine("\t\tCreating Hotel\n");
            Console.Write("\tHotel name: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
                return null;

            Console.Clear();
            Console.WriteLine("\t\tCreating Hotel\n");
            Console.Write("\tHotel address: ");
            string address = Console.ReadLine() ?? string.Empty;

            Hotel hotel = new Hotel(name, address);
            hotel.Id = GetMaxId(hotels);

            hotels.Add(hotel);
            dataHelper.WriteUpdateHotels(hotels);

            return hotel;

        }

        public Hotel ChangeName(Hotel hotel)
        {
            List<Hotel> hotels = GetHotels();

            Console.Clear();
            Console.WriteLine($"\t\tChanging name of hotel {hotel.Name}\n");
            Console.Write("\tHotel name: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("\tName cannot be empty! Name not changed.");
                return hotel;
            }
            if (hotel.Name == name)
            {
                return hotel;
            }

            hotel.Name = name;

            int index = hotels.FindIndex(x => x.Id == hotel.Id);
            if (index == -1)
                hotels.Add(hotel);
            else
                hotels[index] = hotel;

            dataHelper.WriteUpdateHotels(hotels);

            return hotel;
        }

        public bool DeleteHotel(Hotel hotel)
        {
            List<Hotel> hotels = GetHotels();

            int index = hotels.FindIndex(h => h.Id == hotel.Id);
            if (index == -1)
            {
                Console.WriteLine("\t Hotel not found. Press any key to continue...");
                Console.ReadKey();
                return false;

            }
            
            Console.Clear();
            Console.Write($"\tDelete hotel \"{hotel.Name}\" and all data for the hotel? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            dataHelper.DeleteHotelDataBase(hotel.Id);
            hotels.RemoveAt(index);
            dataHelper.WriteUpdateHotels(hotels);

            return true;
        }

        public void EditHotel(Hotel? hotel)
        {
            bool running = true;
            if (hotel == null)
                return;

            Dictionary<int, string[]> menu = GetHotelEditMenu(true);

            while (running)
            {
                if (hotel == null)
                    return;

                // TODO - Print edit hotel header
                Console.Clear();
                Console.WriteLine($"\t\tEdit hotel {hotel.Name}\n\n");
                foreach (var element in menu)
                    Console.WriteLine($"\t{element.Key}. {element.Value[0]}");

                Console.Write("\n\n\tChoose an option: ");
                int choice;
                if (!int.TryParse((Console.ReadLine() ?? "0"), out choice)
                    || !menu.ContainsKey(choice))
                {
                    Console.Clear();
                    Console.WriteLine("\nInvalid option. Press any key to try again.");
                    Console.ReadKey();
                    continue;
                }

                string option = menu[choice][1];

                switch (option)
                {
                    case "220": // Delete hotel
                        DeleteHotel(hotel);
                        running = false;
                        hotel = null;
                        break;
                    case "222": // Change name
                        ChangeName(hotel);
                        break;
                    case "300": // Room managment
                        RoomManagment(hotel);
                        break;
                    case "1000":
                        running = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }

            }
        }

        public void RoomManagment(Hotel hotel)
        {
            bool running = true;

            Room? room;

            Dictionary<int, string[]> menu = GetHotelRoomManagmentMenu(true);

            while (running)
            {
                // TODO - Print Room managment header
                Console.Clear();
                Console.WriteLine($"\t\tEdit rooms in hotel {hotel.Name}\n\n");
                foreach (var element in menu)
                    Console.WriteLine($"\t{element.Key}. {element.Value[0]}");

                Console.Write("\n\n\tChoose an option: ");
                int choice;
                if (!int.TryParse((Console.ReadLine() ?? "0"), out choice)
                    || !menu.ContainsKey(choice))
                {
                    Console.Clear();
                    Console.WriteLine("\nInvalid option. Press any key to try again.");
                    Console.ReadKey();
                    continue;
                }

                string option = menu[choice][1];

                switch (option)
                {

                    case "1": // Show rooms
                        ShowAllRooms(hotel);
                        break;
                    case "2": // Show room types
                        ShowAllRoomTypes(hotel);
                        break;
                    case "3": // Add room
                        AddRoom(hotel);
                        break;
                    case "4": // Edit room
                        room = SelectRoom(hotel);
                        if (room != null)
                            EditRoom(room);
                        room = null;
                        break;
                    case "5": // Delete room
                        room = SelectRoom(hotel);
                        if (room != null)
                            DeleteRoom(room);
                        room = null;
                        break;
                    case "6": // Add room type
                        AddRoomType(hotel);
                        break;
                    case "7": // Edit room type
                        break;
                    case "8": // Delete room type
                        RoomType? roomType = SelectRoomType(hotel);
                        if (roomType != null)
                            DeleteRoomType(roomType);
                        break;
                    case "1000":
                        running = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }

            }
        }

        private Dictionary<int, string[]> GetHotelEditMenu(bool isAdmin)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (isAdmin)
            {
                menu.Add(menu.Count + 1, ["Delete hotel", "220"]);
                menu.Add(menu.Count + 1, ["Change name", "221"]);
                
            }
            menu.Add(menu.Count + 1, ["Room managment", "300"]);
            menu.Add(menu.Count + 1, ["< Back", "1000"]);
            return menu;
        }

        private Dictionary<int, string[]> GetHotelRoomManagmentMenu(bool isAdmin)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count + 1, ["Show rooms", "1"]);
            menu.Add(menu.Count + 1, ["Show room types", "2"]);
            if (isAdmin)
            {
                menu.Add(menu.Count + 1, ["Add room", "3"]);
                menu.Add(menu.Count + 1, ["Edit room", "4"]);
                menu.Add(menu.Count + 1, ["Delete room", "5"]);

                menu.Add(menu.Count + 1, ["Add room type", "6"]);
                menu.Add(menu.Count + 1, ["Edit room type", "7"]);
                menu.Add(menu.Count + 1, ["Delete room type", "8"]);

            }
            menu.Add(menu.Count + 1, ["< Back", "1000"]);

            return menu;
        }

        public void PrintHotelEditHeader(User? admin, Hotel? hotel)
        {
            Console.Clear();

            // Print first row (Hotel, User)
            if (hotel == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Hotel: \"{hotel.Name}\"");

            if (admin == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{admin.Name}\" (0. Logout)");

            Console.WriteLine("\n\n");
        }

        public List<Hotel> GetManageringHotels(int userId)
        {
            List<Hotel> hotels = GetHotels();

            return hotels.Where(h => h.ManagerId == userId).ToList();

        }

        public bool UserIsHotelManager(int userId, int hotelId = 0)
        {
            List<Hotel> hotels = GetManageringHotels(userId);
            if (hotelId == 0)
                return hotels.Count > 0;

            return hotels.Where(h => h.Id == hotelId).Count() > 0;
        }
        //################################ Room #####################################

        public List<Room> GetRooms(int[]? hotelId = null)
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

        public Room? GetRoomById(int id, int hotelId)
        {
            List<Room> rooms = GetRooms([hotelId]);
            return GetRoomById(id, rooms);
        }
        
        public Room? GetRoomById(int id, List<Room> rooms)
        {
            rooms = rooms.Where(x => (x.Id == id)).ToList();
            if (rooms.Count > 0)
                return rooms[0];
            return null;
        }

        public void ShowAllRooms(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tRoom in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms([hotel.Id]);

            int counter = 0;
            foreach (Room r in rooms)
                Console.WriteLine($"\t{++counter}. {r.Presentation()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public Room? AddRoom(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tAdding room in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms();

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

            Console.Clear();
            Console.Write("\tEnter price per night: ");
            string inputText = Console.ReadLine() ?? "0.00";
            decimal pricePerNight;
            if (!decimal.TryParse(inputText, out pricePerNight))
                if ((inputText.Contains('.') && decimal.TryParse(inputText.Replace('.', ','), out pricePerNight))
                    || (inputText.Contains(',') && decimal.TryParse(inputText.Replace(',', '.'), out pricePerNight))) { }
                    


            Console.Clear();
            Console.Write("\tEnter cancellation fee: ");
            inputText = Console.ReadLine() ?? "0.00";
            decimal cancellationFee;
            if (!decimal.TryParse(inputText, out cancellationFee))
                if ((inputText.Contains('.') && decimal.TryParse(inputText.Replace('.', ','), out cancellationFee))
                    || (inputText.Contains(',') && decimal.TryParse(inputText.Replace(',', '.'), out cancellationFee))) { }
                    

            Room room = new Room(number, roomType.Id, hotel.Id, pricePerNight, cancellationFee);
            room.Id = GetMaxId(rooms, hotel.Id);

            rooms.Add(room);
            dataHelper.WriteUpdateHotelRooms(rooms);

            return room;
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
                    Console.WriteLine($"\t{++counter}. {r.Presentation()}");

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

        public void EditRoom(Room room)
        {
            List<Room> rooms = GetRooms();
            List<Room> hotelRooms = rooms.Where(r => r.HotelId == room.HotelId && r.Id != room.Id).ToList();

            RoomType? roomType = GetRoomTypeById(room.RoomTypeId, room.HotelId);
            
            int number = room.Number;
            int roomTypeId = room.RoomTypeId;
            decimal pricePerNight = room.PricePerNight;
            decimal cancellationFee = room.CancellationFee;

            // Edit number
            PrintRoomEditHeader(room);
            Console.Write("\tEdit number? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomEditHeader(room);
                    Console.WriteLine("\tCurrent room number: \"{0}\"", room.Number);
                    Console.Write("\tNew number: ");
                    if (int.TryParse(Console.ReadLine() ?? "", out number))
                    {
                        PrintRoomEditHeader(room);
                        if (number == 0)
                            Console.WriteLine("\tNumber must be greather than \"0\"");
                        else if (hotelRooms.Where(r => r.Id == number).ToList().Count > 0)
                            Console.WriteLine("\tNumber {0} is used!", number);
                        else
                            break;

                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return;
                    }
                }
            }

            // Edit Room type
            PrintRoomEditHeader(room);
            Console.Write("\tEdit room type? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                RoomType? newRoomType = null;

                while (newRoomType == null)
                {
                    PrintRoomEditHeader(room);
                    newRoomType = SelectRoomType(room.HotelId);

                    if (newRoomType == null)
                    {
                        PrintRoomEditHeader(room);
                        Console.WriteLine("\tRoom type cannot be empty!");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            break;
                    }
                    else
                        roomType = newRoomType;
                }
            }

            // Edit pricePerNight
            PrintRoomEditHeader(room);
            Console.Write("\tEdit price per night? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomEditHeader(room);
                    Console.WriteLine("\tCurrent price: \"{0}\"", room.PricePerNight);
                    Console.Write("\tNew price: ");
                    if (decimal.TryParse(Console.ReadLine() ?? "0.00", out pricePerNight))
                    {
                        PrintRoomEditHeader(room);
                        if (pricePerNight == 0m || pricePerNight < 0m)
                        {
                            Console.Write("\tPrice per night is \"{0}\". Continue? (\"Y/n\"): ", pricePerNight);
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                break;
                            else
                                continue;
                        }
                        else if (pricePerNight > 0m)
                            break;
                    }
                }
            }

            // Edit cancellationFee
            PrintRoomEditHeader(room);
            Console.Write("\tEdit cancellation fee? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomEditHeader(room);
                    Console.WriteLine("\tCurrent price: \"{0}\"", room.CancellationFee);
                    Console.Write("\tNew price: ");
                    if (decimal.TryParse(Console.ReadLine() ?? "0.00", out cancellationFee))
                    {
                        PrintRoomEditHeader(room);
                        if (cancellationFee == 0m || cancellationFee < 0m)
                        {
                            Console.Write("\tCancellation fee is \"{0}\". Continue? (\"Y/n\"): ", cancellationFee);
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                break;
                            else
                                continue;
                        }
                        else if (cancellationFee > 0m)
                            break;
                    }
                }
            }

            if ((room.Number == number && room.RoomTypeId == roomTypeId
                && room.PricePerNight == pricePerNight && room.CancellationFee == cancellationFee))
            {
                Console.WriteLine("\tThe room not changed.");
                Console.WriteLine("\tPress any key to continue...");
                Console.ReadKey();
                return;
            }

            room.Number = number;
            room.RoomTypeId = roomTypeId;
            room.PricePerNight = pricePerNight;
            room.CancellationFee = cancellationFee;

            int index = rooms.FindIndex(r => r.Id == room.Id && r.HotelId == room.HotelId);
            if (index ==  -1)
                rooms.Add(room);
            else
                rooms[index] = room;

            dataHelper.WriteUpdateHotelRooms(rooms);

        }

        public bool DeleteRoom(Room room)
        {
            List<Room> rooms = GetRooms();

            int index = rooms.FindIndex(r => r.Id == room.Id);
            if (index == -1)
            {
                return false;
            }

            Console.Clear();
            Console.Write($"\tDelete room \"{room.RoomInfo()}\" and all data for the room? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            dataHelper.DeleteRoomDataBase(room.Id, room.HotelId);
            rooms.RemoveAt(index);
            dataHelper.WriteUpdateHotelRooms(rooms);

            return true;
        }

        public void PrintRoomEditHeader(Room room)
        {
            Console.Clear();

            // Print first row (Room)
            Console.Write($"Edit room: \"{room.Presentation()}\"");

            Console.WriteLine("\n\n");
        }

        //################################ Room type #################################

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
        
        public RoomType? GetRoomTypeById(int id, int hotelId)
        {
            List<RoomType> roomTypes = GetRoomTypes([hotelId]);
            return GetRoomTypeById(id, roomTypes);
        }
        
        public RoomType? GetRoomTypeById(int id, List<RoomType> roomTypes)
        {
            roomTypes = roomTypes.Where(x => (x.Id == id)).ToList();

            if (roomTypes.Count > 0)
                return roomTypes[0];

            return null;
        }

        public void ShowAllRoomTypes(Hotel hotel)
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
            roomType.Id = GetMaxId(roomTypes, hotel.Id);

            roomTypes.Add(roomType);
            dataHelper.WriteUpdateHotelRoomTypes(roomTypes);

            return roomType;
        }

        public RoomType? SelectRoomType(int hotelId)
        {
            Hotel? hotel = GetHotelById(hotelId);
            if (hotel != null)
                return SelectRoomType(hotel);
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

        public void EditRoomType(RoomType roomType)
        {
            List<RoomType> roomTypes = GetRoomTypes();

            string name = roomType.Name;
            List<string> amenities = roomType.Amenities;
            int maximumOccupancy = roomType.MaximumOccupancy;

            // Edit name
            PrintRoomTypeEditHeader(roomType);
            Console.Write("\tEdit name? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(name))
                {
                    PrintRoomTypeEditHeader(roomType);
                    Console.WriteLine("\tCurrent name: \"{0}\"", roomType.Name);
                    Console.Write("New name: ");
                    name = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(name))
                    {
                        PrintRoomTypeEditHeader(roomType);
                        Console.WriteLine("\tName cannot be empty");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return;
                    }
                }
            }

            // TODO amenities

            // Edit maximum occupancy
            PrintRoomTypeEditHeader(roomType);
            Console.Write("\tEdit maximum occupancy? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomTypeEditHeader(roomType);
                    Console.WriteLine("\tCurrent price: \"{0}\"", roomType.MaximumOccupancy);
                    Console.Write("\tNew price: ");
                    if (int.TryParse(Console.ReadLine() ?? "0", out maximumOccupancy))
                    {
                        PrintRoomTypeEditHeader(roomType);
                        if (maximumOccupancy == 0 || maximumOccupancy < 0)
                        {
                            Console.Write("\tMaximum occupancy is \"{0}\". Continue? (\"Y/n\"): ", maximumOccupancy);
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                break;
                            else
                                continue;
                        }
                        else if (maximumOccupancy > 0)
                            break;
                    }
                }
            }

            if (roomType.Name == name && roomType.MaximumOccupancy == maximumOccupancy
                && roomType.Amenities.Count == amenities.Count)
            {
                bool isEquals = true;
                foreach (string amen in amenities)
                {
                    if (!roomType.Amenities.Contains(amen))
                    {
                        isEquals = false;
                        break;
                    }
                }

                if (isEquals)
                {
                    Console.WriteLine("\tThe room not changed.");
                    Console.WriteLine("\tPress any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }

            roomType.Name = name;
            roomType.Amenities = amenities;
            roomType.MaximumOccupancy = maximumOccupancy;
            
            int index = roomTypes.FindIndex(r => r.Id == roomType.Id && r.HotelId == roomType.HotelId);
            if (index == -1)
                roomTypes.Add(roomType);
            else
                roomTypes[index] = roomType;

            dataHelper.WriteUpdateHotelRoomTypes(roomTypes);

        }

        public bool DeleteRoomType(RoomType roomType)
        {
            List<RoomType> roomTypes = GetRoomTypes();

            int index = roomTypes.FindIndex(r => r.Id == roomType.Id);
            if (index == -1)
            {
                return false;
            }

            Console.Clear();
            Console.Write($"\tDelete room \"{roomType.Name}\" and all data for the room? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            dataHelper.DeleteRoomTypeDataBase(roomType.Id, roomType.HotelId);
            roomTypes.RemoveAt(index);
            dataHelper.WriteUpdateHotelRoomTypes(roomTypes);

            return false;
        }
        
        public void PrintRoomTypeEditHeader(RoomType roomType)
        {
            Console.Clear();

            // Print first row (Room)
            Console.Write($"Edit room type: \"{roomType.Name}\"");

            Console.WriteLine("\n\n");
        }

        private int GetMaxId(List<Hotel> list)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Max(x => x.Id);
            return ++maxId;
        }

        private int GetMaxId(List<Room> list, int hotelId)
        {
            if (list.Count == 0)
                return 1;
            
            int maxId = list.Where(r => r.HotelId == hotelId).ToList().Max(x => x.Id);
            return ++maxId;
        }

        private int GetMaxId(List<RoomType> list, int hotelId)
        {
            if (list.Count == 0)
                return 1;
            
            int maxId = list.Where(r => r.HotelId == hotelId).ToList().Max(x => x.Id);
            return ++maxId;
        }
        
    }
}
