using HotelRoomReservationSystem.Helpers;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem
{
    internal class Program
    {
        private static User? user;
        private static Hotel? hotel;
        private static Room? room;
        
        private static DataHelper dataHelper = new DataHelper();
        private static UserHelper userHelper = new UserHelper();
        private static HotelHelper hotelHelper = new HotelHelper();
        private static RoomHelper roomHelper = new RoomHelper();
        private static ReservationHelper reservationHelper = new ReservationHelper();

        private static bool isAdminRegistered = userHelper.isAdminRegistered();

        static void Main(string[] args)
        {
            CreateDatabase();

            CheckAndCancelExpiredReservations();

            MainMenu();

            Environment.Exit(0);

            // TODO
            //
            // User profile menu - view, edit, check user reservations, cancel reservations....
            // ....
        }

        private static void CreateDatabase()
        {
            dataHelper.CreateDataBaseStructure();
        }

        private static void CheckAndCancelExpiredReservations()
        {
            reservationHelper.CheckAndCancelExpiredReservations();
        }

        private static void PrintMenuHeader()
        {
            Console.Clear();

            // Print first row (Hotel, User)
            if (hotel == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Hotel: \"{hotel.Name}\"");

            if (user == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{user.Name}\" (0. Logout)");

            Console.WriteLine("\n\n");
        }

        static Dictionary<int, string[]> GetMainMenu()
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);

            if (!isAdminRegistered)
                menu.Add(menu.Count, ["Create Administrator.", "1"]);
            else
            {
                List<Hotel> hotels = hotelHelper.GetHotels();

                bool isAdmin = (user != null && user.IsAdmin);
                
                if (user != null)
                    menu.Add(menu.Count, ["User profile", "2"]);

                if (isAdmin)
                {
                    if (hotel == null)
                        menu.Add(menu.Count, ["Add hotel", "20"]);
                }

                if (hotels.Count > 0)
                {
                    menu.Add(menu.Count, ["Search menu", "7"]);

                    menu.Add(menu.Count, ["Show hotels", "21"]);
                    if (hotel == null)
                        menu.Add(menu.Count, ["Select hotel", "22"]);
                    else
                    {
                        if (isAdmin)
                        {
                            menu.Add(menu.Count, ["Edit hotel menu", "23"]);
                        }

                        menu.Add(menu.Count, ["Deselect hotel", "24"]);
                        menu.Add(menu.Count, ["Show rooms", "320"]);
                        menu.Add(menu.Count, ["Show room types", "321"]);
                    }

                }
            }
            menu.Add(menu.Count, ["Exit", "1000"]);

            return menu;

        }

        static void MainMenu()
        {
            bool running = true;

            while (running)
            {

                PrintMenuHeader();
                
                Dictionary<int, string[]> menu = GetMainMenu();

                foreach (var element in menu)
                {
                    if (element.Key == 0)
                        continue;

                    Console.WriteLine($"\t{element.Key}. {element.Value[0]}");
                }

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

                if (option == "1")
                {
                    Console.Clear();
                    user = userHelper.AddUser(true);
                    isAdminRegistered = (user != null);
                    continue;
                }

                switch (option)
                {
                    case "0": // login/Logout
                        if (user == null)
                            user = userHelper.UserLogin();
                        else
                            user = null;
                        break;
                    case "2":
                        userHelper.UserProfileMenu(user, null);
                        break;
                    case "7": // Search for room
                        RoomSearch();
                        break;
                    case "20": // add new hotel
                        Hotel? hotel = hotelHelper.AddHotel();
                        if (hotel != null)
                        {
                            Console.Write("\tSelect new hotel? (\"Y/n\"): ");
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                Program.hotel = hotel;
                        }
                        break;
                    case "21": // show hotels
                        hotelHelper.ShowAll();
                        break;
                    case "22": // select hotel
                        Program.hotel = hotelHelper.SelectHotel(Program.hotel);
                        break;
                    case "23": // edit hotel
                        if (Program.hotel != null)
                            hotelHelper.EditHotel(Program.hotel);
                        break;
                    case "24": // deselect hotel
                        Program.hotel = null;
                        break;
                    case "320": // show rooms in selected hotel
                        if (Program.hotel != null)
                            roomHelper.ShowRooms(Program.hotel);
                        break;
                    case "321": // Show room types
                        if (Program.hotel != null)
                            roomHelper.ShowRoomTypes(Program.hotel);
                        break;
                    case "1000":
                        running = false;
                        Console.Clear();
                        break;
                    default:
                        Console.Clear(); 
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }

            }

        }

        static Dictionary<int, string[]> GetSearchMenu()
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);

            menu.Add(menu.Count, ["Search room by date", "30"]);

            if (hotel == null)
            {
                List<Hotel> hotels = hotelHelper.GetHotels();
                if (hotels.Count > 0)
                    menu.Add(menu.Count, ["Select hotel", "22"]);
            }
            else
                menu.Add(menu.Count, ["Deselect hotel", "24"]);

            if (room != null)
                menu.Add(menu.Count, ["Deselect room", "900"]);

            menu.Add(menu.Count, ["< Back", "1000"]);

            return menu;
        }

        static void RoomSearch()
        {
            bool running = true;

            while (running)
            {
                PrintMenuHeader();

                Dictionary<int, string[]> menu = GetSearchMenu();

                foreach (var element in menu)
                {
                    if (element.Key == 0)
                        continue;

                    Console.WriteLine($"\t{element.Key}. {element.Value[0]}");
                }

                Console.Write("\n\n\tChoose an option: ");
                int choice;
                if (!int.TryParse((Console.ReadLine() ?? "-1"), out choice)
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
                    case "0": // login/logout
                        if (user == null)
                            user = userHelper.UserLogin();
                        else
                            user = null;
                        break;
                    case "22": // select hotel
                        hotel = hotelHelper.SelectHotel(hotel);
                        break;
                    case "24": // deselect hotel
                        hotel = null;
                        break;
                    case "30": // Search for available rooms
                        reservationHelper.SearchForAvailableRooms(hotel, user);
                        break;
                    case "900": // deselect room
                        room = null;
                        break;
                    case "1000":
                        running = false;
                        Console.Clear();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }

            }
        }

    }
}
