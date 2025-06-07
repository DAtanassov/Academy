using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class RoomTypeHelper
    {
        public static List<RoomType> GetRoomTypes(int[]? hotelId = null)
        {
            List<RoomType>? roomTypes = DataHelper.GetRoomTypeList();

            if (roomTypes.Count > 0 && hotelId != null)
                roomTypes = roomTypes.Where(x => (hotelId.Contains(x.HotelId))).ToList();

            return roomTypes;
        }

        public static RoomType? GetRoomTypeById(int id, int hotelId)
        {
            List<RoomType> roomTypes = GetRoomTypes([hotelId]);
            return GetRoomTypeById(roomTypes, id);
        }

        private static RoomType? GetRoomTypeById(List<RoomType> roomTypes, int id)
        {
            roomTypes = roomTypes.Where(x => (x.Id == id)).ToList();

            if (roomTypes.Count > 0)
                return roomTypes[0];

            return null;
        }

        public RoomType? SelectRoomType(Hotel hotel)
        {
            List<RoomType> roomTypes = GetRoomTypes([hotel.Id]);

            if (roomTypes.Count == 0)
                return null;

            RoomType? roomType = null;

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            menuHelper.PrintAppName();
            Console.WriteLine("\t\tHotel\n");

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            menuParams.choice = 0;

            Func<string[], string[]> rName = (string[] n) => n;
            Dictionary<int, string[]> menu = roomTypes.Select((val, index) => new { Index = index, Value = val })
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
                            roomType = GetRoomTypeById(roomTypes, int.Parse(menu[menuParams.choice][1]));
                        running = false;
                        break;
                }
            }

            return roomType;
        }

        public static void PrintRoomTypes(Hotel hotel)
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

        public bool AddRoomType(Hotel hotel)
            => AddEditRoomType(new RoomType("", hotel.Id), hotel);

        public bool EditRoomType(RoomType roomType)
            => AddEditRoomType(roomType);

        private bool AddEditRoomType(RoomType roomType, Hotel? hotel = null)
        {
            bool addNew = hotel != null;
            if (!addNew)
                hotel = (new HotelHelper()).GetHotelById(roomType.HotelId);

            if (addNew && hotel != null)
                roomType.HotelId = hotel.Id;

            List<RoomType> roomTypes = GetRoomTypes();
            List<RoomType> hotelRoomTypes = roomTypes.Where(r => r.HotelId == roomType.HotelId && r.Id != roomType.Id).ToList();

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            menuHelper.PrintAppName();
            string title = $"\t\t{(addNew ? "Creat" : "Edit")} room type{((hotel == null) ? "" : (" in " + hotel.ShortInfo()))} \n";
            Console.WriteLine(title);

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            
            bool cancel = false;
            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Name: {roomType.Name}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Maximum occupancy: {roomType.MaximumOccupancy} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? menuParams.prefix : "  ")}3. Add amenities \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 4 ? menuParams.prefix : "  ")}4. Remove amenities \u001b[0m");
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
                                    Console.Write("\tName: ");
                                    roomType.Name = Console.ReadLine() ?? string.Empty;
                                    if (Validator.NameValidate(roomType.Name, roomType.Id, roomType.HotelId, hotelRoomTypes))
                                        break;
                                    else
                                    {
                                        menuHelper.PrintAppName();
                                        Console.WriteLine(title);
                                        if (String.IsNullOrEmpty(roomType.Name))
                                            Console.WriteLine("\tName cannot be empty!");
                                        else
                                            Console.WriteLine("\tName is used!");
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
                                break;
                            case 2:
                                while (true)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.Write("\tMaximum occupancy: ");
                                    if (int.TryParse(Console.ReadLine() ?? "0", out int MaximumOccupancy))
                                    {
                                        if (MaximumOccupancy == 0m || MaximumOccupancy < 0m)
                                        {
                                            menuHelper.PrintAppName();
                                            Console.WriteLine(title);
                                            Console.Write("\tMaximum occupancy is \"{0}\". Continue? (\"Y/n\"): ", MaximumOccupancy);
                                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                                roomType.MaximumOccupancy = MaximumOccupancy;
                                            else
                                                continue;
                                        }
                                        else
                                        {
                                            roomType.MaximumOccupancy = MaximumOccupancy;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case 3:
                                roomType.Amenities = AddAmenities(roomType.Amenities, title);
                                break;
                            case 4:
                                if (roomType.Amenities.Count == 0)
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title); 
                                    Console.WriteLine("\tNo amenities to remove.");
                                    Console.WriteLine("\n\tPress any key to continue...");
                                    Console.ReadKey();
                                    break;
                                }
                                List<string>? amenities = RemoveAmenities(roomType.Amenities, title);
                                if (amenities != null)
                                    roomType.Amenities = amenities;
                                break;
                            case 5:
                                if (!Validator.NameValidate(roomType.Name, roomType.Id, roomType.HotelId, hotelRoomTypes))
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    if (String.IsNullOrEmpty(roomType.Name))
                                        Console.WriteLine("\tName cannot be empty!");
                                    else
                                        Console.WriteLine("\tName is used!");
                                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                                    ConsoleKeyInfo userInput = Console.ReadKey();
                                    if (userInput.Key == ConsoleKey.Escape)
                                    {
                                        cancel = true;
                                        running = false;
                                        break;
                                    }
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
                    DataHelper.InsertHotelRoomTypes([roomType]);
                else
                {
                    int index = roomTypes.FindIndex(r => r.Id == roomType.Id);
                    if (index == -1)
                        roomTypes.Add(roomType);
                    else
                        roomTypes[index] = roomType;
                    DataHelper.UpdateHotelRoomTypes(roomTypes);
                }
            }

            return true;
        }

        private List<string> AddAmenities(List<string> amenities, string title)
        {
            MenuHelper menuHelper = new MenuHelper();

            while (true)
            {
                menuHelper.PrintAppName();
                Console.WriteLine(title);
                if (amenities.Count > 0)
                    Console.WriteLine("\n\tCurrent amenities:\n");
                foreach (string amenity in amenities)
                    Console.WriteLine($"\t\t{amenity}");
                
                Console.Write("\n\tAdd amenity: ");
                string newAmenity = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(newAmenity) && !amenities.Contains(newAmenity.Trim()))
                    amenities.Add(newAmenity.Trim());

                Console.Write($"\n\tAdd more? (\"Y/n\"): ");
                if ((Console.ReadLine() ?? "n").ToLower() != "y")
                    break;
            }

            return amenities;
        }
        
        private List<string>? RemoveAmenities(List<string> amenities, string title)
        {
            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper(); 
            menuHelper.PrintAppName();
            Console.WriteLine(title);
            
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            menuParams.choice = 0;

            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            bool cancel = false;
            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                menu = menuHelper.GetAmenitiesMenu(amenities);

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
                        if (menuParams.choice == menu.Count - 1)
                        {
                            cancel = true;
                            running = false;
                        }
                        if (menuParams.choice == menu.Count - 2)
                            running = false;
                        else if (menuParams.choice == menu.Count - 3)
                        {
                            amenities.Clear();
                            menu.Clear();
                            running = false;
                        }
                        else if (menuParams.choice >= 0 && menuParams.choice < amenities.Count) // Delete specific amenity
                        {
                            amenities.RemoveAt(menuParams.choice);
                            menu.Remove(menuParams.choice);
                        }
                        else
                            running = false;
                        menuHelper.PrintAppName();
                        Console.WriteLine(title);
                        (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                        break;
                }

            }

            Console.CursorVisible = true;

            if (cancel)
                return null;

            return amenities;
        }

        public bool DeleteRoomType(RoomType roomType)
        {
            List<RoomType> roomTypes = GetRoomTypes();

            int index = roomTypes.FindIndex(r => r.Id == roomType.Id);
            if (index == -1)
            {
                return false;
            }

            (new MenuHelper()).PrintAppName();
            Console.Write($"\tDelete room type \"{roomType.Name}\" and all data for the room? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            DataHelper.DeleteRoomTypeData(roomType.Id);
            DataHelper.DeleteRoomTypes([roomType]);

            return false;
        }

    }
}
