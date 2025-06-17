using HotelRoomReservationSystem.DB.JSON;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class HotelHelper
    {   
        private readonly static DBService<Hotel> hotelDBService = new DBService<Hotel>(new HotelDB());

        public static List<Hotel> GetHotels()
            => hotelDBService.GetList();

        public static List<Hotel> GetHotels(int[] hotelId)
        {
            List<Hotel> hotels = GetHotels();

            if (hotelId.Length > 0)
                hotels = hotels.Where(h => hotelId.Contains(h.Id)).ToList();

            return hotels;
        }
        
        public Hotel? GetHotelById(int hotelId)
        {
            List<Hotel> hotels = GetHotels([hotelId]);
            return GetHotelById(hotels, hotelId);
        }

        public Hotel? GetHotelById(List<Hotel> hotels, int id)
        {
            if (hotels.Count == 0)
                return null;
            return hotels.FirstOrDefault(h => h.Id == id);
        }

        public Hotel? SelectHotel(Hotel? hotel = null, User? user = null)
        {
            List<Hotel> hotels = GetHotels();

            if (hotels.Count > 0 && user != null && !user.IsAdmin)
                hotels = hotels.Where(h => h.ManagerId == user.Id).ToList();

            if (hotels.Count == 0)
                return hotel;
            
            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            menuHelper.PrintAppName();
            Console.WriteLine("\t\tHotels\n");

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            Func<string[], string[]> hotelName = (string[] n) => n;
            Dictionary<int, string[]> menu = hotels.Select((val, index) => new { Index = index, Value = val })
                                                    .ToDictionary(h => h.Index, h => hotelName([h.Value.Name, h.Value.Id.ToString()]));
            menu.Add(menu.Count, ["Cancel","0"]);
            for (int i = menu.Count; i > 0; i--)
            {
                menu.Add(i, menu[i - 1]);
                menu.Remove(i - 1);
            }

            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                menuHelper.PrintMenuElements(menu, menuParams, false);
                
                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 1 ? menu.Count : menuParams.choice - 1;
                        continue;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == menu.Count ? 1 : menuParams.choice + 1;
                        continue;

                    case ConsoleKey.Enter:
                        if (menuParams.choice != menu.Count)
                            hotel = GetHotelById(hotels, int.Parse(menu[menuParams.choice][1]));
                        running = false;
                        break;
                }
            }

            return hotel;
        }

        public void PrintHotels()
        {
            (new MenuHelper()).PrintAppName();
            Console.WriteLine("\t\tHotels\n");

            List<Hotel> hotels = GetHotels();

            int counter = 0;
            foreach (Hotel h in hotels)
                Console.WriteLine($"\t{++counter}. {h.Info()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public bool AddHotel() => AddEditHotel(new Hotel(), true);

        public bool EditHotel(Hotel hotel) => AddEditHotel(hotel);

        private bool AddEditHotel(Hotel hotel, bool addNew = false)
        {
            User? user = null;

            List<Hotel> hotels = GetHotels();

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            UserHelper userHelper = new UserHelper();

            menuHelper.PrintAppName();
            string title = $"\t\t{(addNew ? "Creat" : "Edit")} Hotel\n";
            Console.WriteLine(title);

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            bool cancel = false;
            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                if (hotel.ManagerId != 0)
                    user = userHelper.GetUserById(hotel.ManagerId);
                else
                    user = null;

                Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Name: {hotel.Name}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Address: {hotel.Address} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? menuParams.prefix : "  ")}3. Menager: {(user == null ? "" : user.Name)} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 4 ? menuParams.prefix : "  ")}4. Save\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 5 ? menuParams.prefix : "  ")}5. Cancel\u001b[0m");

                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 1 ? 5 : menuParams.choice - 1;
                        continue;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == 5 ? 1 : menuParams.choice + 1;
                        continue;

                    case ConsoleKey.Enter:

                        Console.CursorVisible = true;

                        switch (menuParams.choice)
                        {
                            case 1:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tHotel name: ");
                                hotel.Name = Console.ReadLine() ?? string.Empty;
                                while (!Validator.NameValidate(hotel.Name, 0, hotels))
                                {
                                    if (string.IsNullOrEmpty(hotel.Name))
                                        Console.WriteLine("\tName cannot be empty!");
                                    else
                                        Console.WriteLine("\tName is used!");
                                    Console.Write("\tName: ");
                                    hotel.Name = Console.ReadLine() ?? string.Empty;
                                }
                                break;
                            case 2:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title); 
                                Console.Write("\tHotel address: ");
                                hotel.Address = Console.ReadLine() ?? string.Empty;
                                break;
                            case 3:
                                user = userHelper.SelectUser(Program.user, user);
                                if (user != null)
                                    hotel.ManagerId = user.Id;
                                break;
                            case 4:
                                if (string.IsNullOrWhiteSpace(hotel.Name))
                                {
                                    Console.WriteLine("\tHotel name cannot be empty. Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                                }
                                running = false;
                                break;
                            case 5:
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
                    hotelDBService.Insert(hotel);
                else
                    hotelDBService.Update(hotel);
            }
            return true;
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

            hotelDBService.Delete(hotel);

            return true;
        }

        private static List<Hotel> GetManageringHotels(int userId)
        {
            List<Hotel> hotels = GetHotels();

            return hotels.Where(h => h.ManagerId == userId).ToList();

        }

        public static bool UserIsHotelManager(int userId, int hotelId = 0)
        {
            List<Hotel> hotels = GetManageringHotels(userId);
            if (hotelId == 0)
                return hotels.Count > 0;

            return hotels.Where(h => h.Id == hotelId).Count() > 0;
        }

    }
}
