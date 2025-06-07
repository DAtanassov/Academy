using HotelRoomReservationSystem.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelRoomReservationSystem.Helpers
{
    public class RoomHelper
    {
        public static List<Room> GetRooms(int[]? hotelId = null)
        {
            List<Room>? rooms = DataHelper.GetRoomList();

            if (rooms.Count > 0 && hotelId != null)
                rooms = rooms.Where(x => hotelId.Contains(x.HotelId)).ToList();

            return rooms;
        }

        public static Room? GetRoomById(int id, int hotelId)
        {
            List<Room> rooms = GetRooms([hotelId]);
            return GetRoomById(rooms, id);
        }

        private static Room? GetRoomById(List<Room> rooms, int id)
        {
            if (rooms.Count == 0)
                return null;
            return rooms.FirstOrDefault(r => r.Id == id);
        }
        
        public Room? SelectRoom(Hotel hotel)
        {
            List<Room> rooms = GetRooms([hotel.Id]);

            if (rooms.Count == 0)
                return null;

            Room? room = null;

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            menuHelper.PrintAppName();
            Console.WriteLine("\t\tHotels\n");

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            menuParams.choice = 0;

            Func<string[], string[]> rName = (string[] n) => n;
            Dictionary<int, string[]> menu = rooms.Select((val, index) => new { Index = index, Value = val })
                                                    .ToDictionary(r => r.Index, r => rName([r.Value.ShortInfo(), r.Value.Id.ToString()]));
            menu.Add(menu.Count, ["Cancel", "0"]);

            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                menuHelper.PrintMenuElements(menu, menuParams, false);

                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 0 ? menu.Count - 1 : menuParams.choice - 1;
                        continue;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == menu.Count - 1 ? 0 : menuParams.choice + 1;
                        continue;

                    case ConsoleKey.Enter:
                        if (menuParams.choice != menu.Count - 1)
                            room = GetRoomById(rooms, int.Parse(menu[menuParams.choice][1]));
                        running = false;
                        break;
                }
            }

            return room;
        }

        public static void PrintRooms(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tRoom in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms([hotel.Id]);

            int counter = 0;
            foreach (Room r in rooms)
                Console.WriteLine($"\t{++counter}. {r.ShortInfo()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public bool AddRoom(Hotel hotel)
            => AddEditRoom(new Room(0, 0, hotel.Id), hotel);

        public bool EditRoom(Room room)
            => AddEditRoom(room);
        
        private bool AddEditRoom(Room room, Hotel? hotel = null)
        {
            bool addNew = hotel != null;
            if (!addNew)
                hotel = (new HotelHelper()).GetHotelById(room.HotelId);
            
            if (addNew && hotel != null)
                room.HotelId = hotel.Id;

            List<Room> rooms = GetRooms();
            List<Room> hotelRooms = rooms.Where(r => r.HotelId == room.HotelId && r.Id != room.Id).ToList();

            RoomType? roomType = addNew ? null : RoomTypeHelper.GetRoomTypeById(room.RoomTypeId, room.HotelId);

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            RoomTypeHelper roomTypeHelper = new RoomTypeHelper();

            menuHelper.PrintAppName();
            string title = $"\t\t{(addNew ? "Creat" : "Edit")} room{((hotel == null) ? "" : (" in " + hotel.ShortInfo()))} \n";
            Console.WriteLine(title);

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            bool cancel = false;
            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Number: {room.Number}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Room type:  {((roomType == null) ? "" : roomType.ShortInfo())} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? menuParams.prefix : "  ")}3. Price per night:  {room.PricePerNight} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 4 ? menuParams.prefix : "  ")}4. Cancellation fee:  {room.CancellationFee} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 5 ? menuParams.prefix : "  ")}5. Save\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 6 ? menuParams.prefix : "  ")}6. Cancel\u001b[0m");

                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 1 ? 6 : menuParams.choice - 1;
                        continue;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == 6 ? 1 : menuParams.choice + 1;
                        continue;

                    case ConsoleKey.Enter:

                        Console.CursorVisible = true;

                        switch (menuParams.choice)
                        {
                            case 1:
                                while (true)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.Write("\tNumber: ");
                                    if (int.TryParse(Console.ReadLine() ?? "", out int number))
                                     {

                                        if (Validator.RoomNumberValidate(number, room.Id, room.HotelId, hotelRooms))
                                        {
                                            room.Number = number;
                                            break;
                                        }
                                        else
                                        {
                                            menuHelper.PrintAppName();
                                            Console.WriteLine(title);
                                            if (number == 0)
                                                Console.WriteLine("\tNumber must be greather than \"0\"");
                                            else 
                                                Console.WriteLine("\tNumber {0} is used!", number);

                                            Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                                            ConsoleKeyInfo userInput = Console.ReadKey();
                                            if (userInput.Key == ConsoleKey.Escape)
                                            {
                                                cancel = true;
                                                running = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case 2:
                                roomType = roomTypeHelper.SelectRoomType(hotel);
                                while (roomType == null)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.WriteLine("\tRoom type cannot be empty!");
                                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                                    ConsoleKeyInfo userInput = Console.ReadKey();
                                    if (userInput.Key == ConsoleKey.Escape)
                                    {
                                        cancel = true;
                                        running = false;
                                        break;
                                    }
                                    roomType = roomTypeHelper.SelectRoomType(hotel);
                                }
                                if (roomType != null)
                                    room.RoomTypeId = roomType.Id;
                                break;
                            case 3:
                                while (true)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.Write("\tPrice per night: ");
                                    if (decimal.TryParse(Console.ReadLine() ?? "0.00", out decimal pricePerNight))
                                    {
                                        if (pricePerNight == 0m || pricePerNight < 0m)
                                        {
                                            menuHelper.PrintAppName();
                                            Console.WriteLine(title);
                                            Console.Write("\tPrice per night is \"{0}\". Continue? (\"Y/n\"): ", pricePerNight);
                                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                                room.PricePerNight = pricePerNight;
                                            else
                                                continue;
                                        }
                                        else 
                                        {
                                            room.PricePerNight = pricePerNight;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 4:
                                while (true)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.Write("\tCancellation fee: ");
                                    if (decimal.TryParse(Console.ReadLine() ?? "0.00", out decimal cancellationFee))
                                    {
                                        if (cancellationFee == 0m || cancellationFee < 0m)
                                        {
                                            menuHelper.PrintAppName();
                                            Console.WriteLine(title);
                                            Console.Write("\tCancellation fee is \"{0}\". Continue? (\"Y/n\"): ", cancellationFee);
                                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                                room.CancellationFee = cancellationFee;
                                            else
                                                continue;
                                        }
                                        else
                                        {
                                            room.CancellationFee = cancellationFee;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 5:
                                if (room.RoomTypeId == 0)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.WriteLine("\tRoom type cannot be empty!");
                                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                                    ConsoleKeyInfo userInput = Console.ReadKey();
                                    if (userInput.Key == ConsoleKey.Escape)
                                    {
                                        cancel = true;
                                        running = false;
                                    }
                                    break;
                                }
                                if (!Validator.RoomNumberValidate(room.Number, room.Id, room.HotelId, hotelRooms))
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.WriteLine("\tRoom number is not valid!");
                                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                                    ConsoleKeyInfo userInput = Console.ReadKey();
                                    if (userInput.Key == ConsoleKey.Escape)
                                    {
                                        cancel = true;
                                        running = false;
                                    }
                                    break;
                                }
                                running = false;
                                break;
                            case 6:
                                cancel = true;
                                running = false;
                                break;

                        }
                        Console.CursorVisible = false;
                        menuHelper.PrintAppName();
                        Console.WriteLine(title);
                        (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                        break;
                }
            }
            
            if (cancel)
                return false;
            else
            {
                if (addNew)
                    DataHelper.InsertHotelRooms([room]);
                else
                {
                    int index = rooms.FindIndex(r => r.Id == room.Id);
                    if (index == -1)
                        rooms.Add(room);
                    else
                        rooms[index] = room;
                    DataHelper.UpdateHotelRooms(rooms);
                }
            }
            return true;
        }

        public bool DeleteRoom(Room room)
        {
            List<Room> rooms = GetRooms();

            int index = rooms.FindIndex(r => r.Id == room.Id);
            if (index == -1)
            {
                return false;
            }

            (new MenuHelper()).PrintAppName();
            Console.Write($"\tDelete room:\n\"{room.Info()}\" and all data for the room? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            DataHelper.DeleteRoomData(room.Id);
            DataHelper.DeleteRooms([room]);

            return true;
        }

    }
}
