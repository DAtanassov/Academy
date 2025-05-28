using System.ComponentModel.Design;
using System.Xml.Linq;
using HotelRoomReservationSystem.Helpers;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem
{
    internal class Program
    {
        private static User? user;

        private static ReservationHelper reservationHelper = new ReservationHelper();

        static void Main(string[] args)
        {
            // Creating database files if not exist
            DataHelper.CreateDataBase();

            while (!UserHelper.isAdminRegistered())
            {
                Console.WriteLine("\n\n");
                Console.WriteLine("\t1. Create Administrator");
                Console.WriteLine("\t2. Exit");

                Console.Write("\n\n\tChoose an option: ");
                int choice;
                if (!int.TryParse((Console.ReadLine() ?? "0"), out choice)
                    || !(choice == 1 || choice == 2))
                {
                    Console.Clear();
                    Console.Write("\nInvalid option. Press any key to try again.");
                    Console.ReadKey();
                    continue;
                }
                else if (choice == 1)
                    user = UserHelper.AddUser(true);
                else if (choice == 2)
                    Environment.Exit(0);
            }

            ReservationHelper.CheckAndCancelExpiredReservations();

            MainMenu();

            Environment.Exit(0);
        }

        // ############################# Main menu ####################################
        private static void PrintMainMenuHeader()
        {
            Console.Clear();

            // Print first row (User)
            Console.Write($"\t\t");
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

            bool hotelManager = (user != null && HotelHelper.UserIsHotelManager(user.Id));
            bool isAdmin = (user != null && user.IsAdmin);

            menu.Add(menu.Count, ["Search", "1"]);
            if (user != null)
                menu.Add(menu.Count, ["User profile", "2"]);
            if (isAdmin || hotelManager)
            {
                if (!hotelManager)
                    menu.Add(menu.Count, ["Users managment", "3"]);
                menu.Add(menu.Count, ["Hotels managment", "4"]);
                menu.Add(menu.Count, ["Reservations managment", "5"]);
            }
            menu.Add(menu.Count, ["Exit", "1000"]);

            return menu;
        }

        static void MainMenu()
        {
            bool running = true;

            while (running)
            {

                PrintMainMenuHeader();
                
                Dictionary<int, string[]> menu = GetMainMenu();

                foreach (var element in menu)
                {
                    if (element.Key == 0)
                        continue;
                    Console.WriteLine($"\t{element.Key}. {element.Value[0]}");
                }

                int choice;
                Console.Write("\n\n\tChoose an option: ");
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
                    case "0": // login/Logout
                        if (user == null)
                            user = UserHelper.UserLogin();
                        else
                        {
                            PrintMainMenuHeader();
                            Console.Write("\tAre you sure to logout? (\"Y/n\"): ");
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                user = null;
                        }
                        break;
                    case "1": // Search
                        SearchMenu();
                        break;
                    case "2": // User profile
                        UserProfileMenu(null);
                        break;
                    case "3": // Users managment
                        if (user != null)
                            UsersManagmentMenu(null);
                        break;
                    case "4": // Hotels managment
                        if (user != null)
                            HotelManagmentMenu();
                        break;
                    case "5": // Reservations managment
                        ReservationsManagmentMenu(user, null);
                        break;
                    case "1000": // Exit
                        PrintMainMenuHeader();
                        Console.Write("\tExit application? (\"Y/n\"): ");
                        if ((Console.ReadLine() ?? "n").ToLower() != "y")
                            continue;
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

        // ############################# Search menu ##################################
        private static void PrintSearchMenuHeader(Hotel? hotel, DateTime? checkInDate, DateTime? checkOutDate, string? location)
        {
            Console.Clear();

            // Print first row (Hotel/location, User)
            if (hotel == null)
                if (!string.IsNullOrEmpty(location))
                    Console.Write($"Search in location: {location.Trim()}");
                else
                    Console.Write($"Search in all hotels");
            else
                Console.Write($"Search in hotel: \"{hotel.Name}\"");

            if (user == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{user.Name}\" (0. Logout)");

            // Print Second row check-in/out
            Console.WriteLine("\n");
            Console.Write($"Check-in: {((checkInDate == null) ? "<not selected>" : ((DateTime)checkInDate).ToShortDateString())}");
            Console.Write($"\tCheck-out: {((checkOutDate == null) ? "<not selected>" : ((DateTime)checkOutDate).ToShortDateString())}");

            Console.WriteLine("\n\n");
        }

        private static void SearchMenu()
        {
            Hotel? hotel = null;
            string location = "";
            DateTime checkInDate = new DateTime();
            DateTime checkOutDate = new DateTime();
            int choice;

            while (true)
            {
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);

                Console.WriteLine("\t1. Search in hotel");
                Console.WriteLine("\t2. Serch by location");
                Console.WriteLine("\t3. < Back");
                Console.WriteLine("\t4. Exit");

                Console.Write("\n\n\tChoose an option: ");
                if (!int.TryParse((Console.ReadLine() ?? "0"), out choice)
                    || !(choice == 1 || choice == 2 || choice == 3 || choice == 4))
                {
                    Console.Clear();
                    Console.Write("\nInvalid option. Press any key to try again.");
                    Console.ReadKey();
                    continue;
                }
                else if (choice == 1)
                {
                    hotel = HotelHelper.SelectHotel();
                    break;
                }
                else if (choice == 2)
                {
                    PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                    Console.Write("\tEnter location: ");
                    location = Console.ReadLine() ?? string.Empty;
                    break;
                }
                else if (choice == 3)
                    return;
                else if (choice == 4)
                {
                    PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                    Console.Write("\tExit application? (\"Y/n\"): ");
                    if ((Console.ReadLine() ?? "n").ToLower() != "y")
                        continue;
                    Environment.Exit(0);
                }
                    
            }

            PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
            Console.Write("\tEnter check in date: ");
            while (!DateTime.TryParse(Console.ReadLine(), out checkInDate)
                || checkInDate <= DateTime.Today)
            {
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                Console.WriteLine("\tCheck-in date must be greater than current!");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                Console.Write("\tEnter check in date: ");
            }

            PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
            Console.Write("\tEnter check out date: ");
            while (!DateTime.TryParse(Console.ReadLine(), out checkOutDate)
                || checkOutDate <= DateTime.Today
                || checkOutDate < checkInDate)
            {
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                Console.WriteLine("\tCheck-out date must be greater than current and check-in date!");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                Console.Write("\tEnter check out date: ");
            }

            List<Room> rooms = reservationHelper.SearchForAvailableRooms(hotel, checkInDate, checkOutDate, location);

            List<Hotel> hotels;
            if (hotel == null)
                hotels = HotelHelper.GetHotels();
            else
                hotels = new List<Hotel> { hotel };

            Dictionary<int, Room> menu = new Dictionary<int, Room>();

            PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
            if (rooms.Count == 0)
            {
                Console.WriteLine("\n\t Rooms not found");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to search again.");
                ConsoleKeyInfo userInput = Console.ReadKey(true);
                if (userInput.Key != ConsoleKey.Escape)
                    SearchMenu();

                return;
            }

            Console.WriteLine("\tAvailable rooms:\n");
            int counter = 0;
            if (hotel == null)
            {
                int[] hotelsId = rooms.Select(r => r.HotelId).ToArray();
                hotels = hotels.Where(h => hotelsId.Contains(h.Id)).OrderBy(o => o.Name).ToList();
                foreach (Hotel h in hotels)
                {
                    Console.WriteLine($"\tHotel \"{h.ShortInfo()}\"");

                    List<Room> hRooms = rooms.Where(r => r.HotelId == h.Id).OrderBy(o => o.Number).ToList();
                    foreach (Room r in hRooms)
                    {
                        Console.WriteLine($"\t\t{++counter}. {r.ShortInfo()}");
                        menu.Add(counter, r);
                    }
                }
            }
            else
            {
                rooms = rooms.OrderBy(o => o.HotelId).ThenBy(o => o.Number).ToList();
                foreach (Room r in rooms)
                {
                    Console.WriteLine($"\t{++counter}. {r.ShortInfo()}");
                    menu.Add(counter, r);
                }

            }

            Console.WriteLine($"\t{++counter}. Cancel");
            Console.Write("\n\tChoose a room: ");
            if (!int.TryParse((Console.ReadLine() ?? "-1"), out choice)
                || !(menu.ContainsKey(choice) || choice == counter))
            {
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                Console.WriteLine("\nInvalid option. Press any key to try again.");
                Console.ReadKey();
                return;
            }

            if (choice == counter)
                return;

            while (user == null)
            {
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);

                Console.Clear();
                Console.Write("\n\tUsername: ");
                string username = Console.ReadLine() ?? "";
                Console.Clear();
                Console.Write("\n\tPassword: ");
                string password = Console.ReadLine() ?? "";
                user = UserHelper.GetUser(username, password);
                if (user == null)
                {
                    PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                    Console.WriteLine("\tUser not logged in!");
                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to try again.");
                    ConsoleKeyInfo userInput = Console.ReadKey(true);
                    if (userInput.Key == ConsoleKey.Escape)
                        return;
                }

            }

            Room room = menu[choice];

            PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
            Console.WriteLine($"\tSelected room:\n" +
                              $"{room.Info()}");

            Console.WriteLine($"\n\t1. Book the room\n" +
                                $"\t2. Search again\n" +
                                $"\t3. Cancel");

            Console.Write("\n\tChoose an option: ");
            choice = 0;
            while (!int.TryParse((Console.ReadLine() ?? "0"), out choice)
                || !(choice == 1 || choice == 2 || choice == 3))
            {
                PrintSearchMenuHeader(hotel, checkInDate, checkOutDate, location);
                Console.WriteLine("\nInvalid option. Press any key to try again.");
                Console.ReadKey();
            }

            if (choice == 1)
                reservationHelper.BookTheRoom(room, (User)user, checkInDate, checkOutDate, location);
            else if (choice == 2)
                SearchMenu();

        }

        // ############################# Users managment ##############################

        public static void PrintUserProfileHeader(Reservation? reservation)
        {
            Console.Clear();

            // Print first row (reservation, User)
            if (reservation == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Edit reservation ID: \"{reservation.Id.ToString()}\"");

            if (user == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{user.Name}\" (0. Logout)");

            Console.WriteLine("\n\n");
        }

        private static Dictionary<int, string[]> GetUserProfileMenu(Reservation? reservation)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);

            if (user != null)
            {
                menu.Add(menu.Count, ["View profile", "1"]);
                menu.Add(menu.Count, ["Edit profile", "2"]);
                if (!user.IsAdmin)
                    menu.Add(menu.Count, ["Delete profile", "3"]);
                menu.Add(menu.Count, ["Reservations menu", "4"]);
            }
            menu.Add(menu.Count, ["< Back", "1000"]);

            return menu;
        }

        private static void UserProfileMenu(Reservation? reservation)
        {
            bool running = true;

            while (running)
            {
                PrintUserProfileHeader(reservation);

                Dictionary<int, string[]> menu = GetUserProfileMenu(reservation);

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

                switch (option)
                {
                    case "0": // login/Logout
                        if (user == null)
                            user = UserHelper.UserLogin();
                        else
                        {
                            PrintUserProfileHeader(reservation);
                            Console.Write("\tAre you sure to logout? (\"Y/n\"): ");
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                user = null;
                        }
                        if (user != null && !user.IsAdmin)
                            running = false;
                        break;
                    case "1": // View profile
                        if (user != null)
                        {
                            PrintUserProfileHeader(reservation);
                            Console.WriteLine(user.Info());
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "2": // Edit profile
                        if (user != null && UserHelper.EditUser(user, user))
                        {
                            PrintUserProfileHeader(reservation);
                            Console.WriteLine("\tProfile edited successfully!");
                            user.Info();
                        }
                        else
                        {
                            PrintUserProfileHeader(reservation);
                            Console.WriteLine("\tProfile not changed.");
                        }
                        Console.WriteLine("\n\tPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "3": // Delete profile
                        if (user != null)
                        {
                            PrintUserProfileHeader(reservation);
                            string name = user.Name;
                            UserHelper.DeleteUser(user);
                            running = false;
                            user = null;
                            reservation = null;
                            Console.WriteLine($"\tUser \"{name}\" delete successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "4": // Reservations menu
                        ReservationsManagmentMenu(user, reservation);
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

        private static Dictionary<int, string[]> GetUsersManagmentMenu(User? sUser, Reservation? reservation)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);

            if (user != null)
            {
                if (user.IsAdmin || (sUser != null && user.Id == sUser.Id))
                {
                    menu.Add(menu.Count, ["View users", "1"]);
                    menu.Add(menu.Count, ["Add user", "2"]);

                    if (sUser == null)
                        menu.Add(menu.Count, ["Edit user", "3"]);
                    else
                    {
                        menu.Add(menu.Count, ["Deselect user", "4"]);
                        menu.Add(menu.Count, ["Edit another user", "5"]);
                        menu.Add(menu.Count, ["Delete user", "6"]);
                        menu.Add(menu.Count, ["Reservations menu", "7"]);
                    }
                }
            }

            menu.Add(menu.Count, ["< Back", "999"]);
            menu.Add(menu.Count, ["Exit", "1000"]);

            return menu;
        }

        private static void UsersManagmentMenu(Reservation? reservation)
        {
            bool running = true;

            User? sUser = null;

            while (running)
            {
                UserHelper.PrintUserManagmentHeader(user, sUser, reservation);

                Dictionary<int, string[]> menu = GetUsersManagmentMenu(sUser, reservation);

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

                switch (option)
                {
                    case "0": // login/Logout
                        if (user == null)
                            user = UserHelper.UserLogin();
                        else
                        {
                            UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                            Console.Write("\tAre you sure to logout? (\"Y/n\"): ");
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                user = null;
                        }
                        break;
                    case "1": // View users
                        UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                        UserHelper.ShowAll(user);
                        break;
                    case "2": // Add user
                        UserHelper.AddUser(false);
                        break;
                    case "3": // Edit user
                        if (user != null && sUser == null)
                        {
                            UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                            sUser = UserHelper.SelectUser(user, sUser);
                        }
                        if (user != null && sUser != null)
                        {
                            if (UserHelper.EditUser(user, sUser))
                            {
                                UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                                Console.WriteLine("\tProfile edited successfully!");
                                sUser.Info();
                            }
                            else
                            {
                                UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                                Console.WriteLine("\tProfile not changed.");
                            }
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "4": // Deselect user
                        sUser = null;
                        break;
                    case "5": // Edit another user
                        if (user != null)
                        {
                            UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                            sUser = UserHelper.SelectUser(user, sUser);
                            if (sUser != null)
                            {
                                if (UserHelper.EditUser(user, sUser))
                                {
                                    UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                                    Console.WriteLine("\tProfile edited successfully!");
                                    sUser.Info();
                                }
                                else
                                {
                                    UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                                    Console.WriteLine("\tProfile not changed.");
                                }
                                Console.WriteLine("\n\tPress any key to continue...");
                                Console.ReadKey();
                            }
                        }
                        break;
                    case "6": // Delete user
                        if (sUser != null && user != sUser)
                        {
                            UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                            string name = sUser.Name;
                            UserHelper.DeleteUser(sUser);
                            sUser = null;
                            reservation = null;
                            Console.WriteLine($"\tUser \"{name}\" delete successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "7": // Reservations menu
                        ReservationsManagmentMenu(sUser, reservation);
                        break;
                    case "999":
                        running = false;
                        Console.Clear();
                        break;
                    case "1000":
                        UserHelper.PrintUserManagmentHeader(user, sUser, reservation);
                        Console.Write("\tExit application? (\"Y/n\"): ");
                        if ((Console.ReadLine() ?? "n").ToLower() != "y")
                            continue;
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ############################# Hotels managment #############################

        public static void PrintHotelEditHeader(Hotel? hotel)
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

        private static Dictionary<int, string[]> GetHotelManagmentMenu()
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);
            menu.Add(menu.Count, ["View hotels", "1"]);

            bool hotelManager = (user != null && HotelHelper.UserIsHotelManager(user.Id));
            bool isAdmin = (user != null && user.IsAdmin);
            
            if (isAdmin || hotelManager)
            {
                if (!hotelManager)
                    menu.Add(menu.Count, ["Add hotel", "2"]);
                menu.Add(menu.Count, ["Edit hotel", "3"]);
                if (!hotelManager)
                    menu.Add(menu.Count, ["Delete hotel", "4"]);
            }
            
            menu.Add(menu.Count, ["< Back", "999"]);
            menu.Add(menu.Count, ["Exit", "1000"]);

            return menu;
        }

        private static void HotelManagmentMenu()
        {
            bool running = true;

            Hotel? hotel = null;

            while (running)
            {
                PrintHotelEditHeader(hotel);

                Dictionary<int, string[]> menu = GetHotelManagmentMenu();

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

                switch (option)
                {
                    case "0": // login/Logout
                        if (user == null)
                            user = UserHelper.UserLogin();
                        else
                        {
                            PrintHotelEditHeader(hotel);
                            Console.Write("\tAre you sure to logout? (\"Y/n\"): ");
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                user = null;
                        }
                        if (user != null && !user.IsAdmin)
                            running = false;
                        break;
                    case "1": // View hotels
                        HotelHelper.ShowAll();
                        break;
                    case "2": // Add hotel
                        hotel = HotelHelper.AddHotel();
                        break;
                    case "3": // Edit hotel
                        PrintHotelEditHeader(hotel);
                        hotel = HotelHelper.SelectHotel();
                        if (hotel != null)
                            EditHotel(hotel);
                        hotel = null;
                        break;
                    case "4": // Delete hotel
                        PrintHotelEditHeader(hotel);
                        hotel = HotelHelper.SelectHotel();
                        if (hotel != null)
                        {
                            if (HotelHelper.DeleteHotel(hotel))
                            {
                                // TODO - Message
                                hotel = null;
                            }
                            else
                            {
                                // TODO - Message
                            }
                        }
                        break;
                    case "999": // < Back
                        running = false;
                        Console.Clear();
                        break;
                    case "1000":
                        PrintHotelEditHeader(hotel);
                        Console.Write("\tExit application? (\"Y/n\"): ");
                        if ((Console.ReadLine() ?? "n").ToLower() != "y")
                            continue;
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static Dictionary<int, string[]> GetHotelEditMenu(bool isAdmin)
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

        public static void EditHotel(Hotel? hotel)
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
                        if (HotelHelper.DeleteHotel(hotel))
                        {
                            // TODO - Message
                            running = false;
                            hotel = null;
                        }
                        break;
                    case "222": // Change name
                        hotel = HotelHelper.ChangeName(hotel);
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

        // ############################# Room managment ###############################

        private static Dictionary<int, string[]> GetHotelRoomManagmentMenu(bool isAdmin)
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

        public static void RoomManagment(Hotel hotel)
        {
            bool running = true;

            Room? room;
            RoomType? roomType;

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
                        RoomHelper.ShowAllRooms(hotel);
                        break;
                    case "2": // Show room types
                        RoomTypeHelper.ShowAllRoomTypes(hotel);
                        break;
                    case "3": // Add room
                        room = RoomHelper.AddRoom(hotel);
                        break;
                    case "4": // Edit room
                        room = RoomHelper.SelectRoom(hotel);
                        if (room != null)
                            RoomHelper.EditRoom(room);
                        room = null;
                        break;
                    case "5": // Delete room
                        room = RoomHelper.SelectRoom(hotel);
                        if (room != null)
                            RoomHelper.DeleteRoom(room);
                        room = null;
                        break;
                    case "6": // Add room type
                        roomType = RoomTypeHelper.AddRoomType(hotel);
                        break;
                    case "7": // Edit room type
                        roomType = RoomTypeHelper.SelectRoomType(hotel);
                        if (roomType != null) 
                            RoomTypeHelper.EditRoomType(roomType);
                        break;
                    case "8": // Delete room type
                        roomType = RoomTypeHelper.SelectRoomType(hotel);
                        if (roomType != null)
                            RoomTypeHelper.DeleteRoomType(roomType);
                        roomType = null;
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

        // ############################# Reservations managment #######################

        private static Dictionary<int, string[]> GetReservationsManagmentMenu()
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);

            bool hotelManager = (user != null && HotelHelper.UserIsHotelManager(user.Id));
            bool isAdmin = (user != null && user.IsAdmin);

            if (user != null)
            {
                menu.Add(menu.Count, ["View reservations", "1"]);
                menu.Add(menu.Count, ["View history of reservations", "2"]);
                if (user.IsAdmin)
                {
                    menu.Add(menu.Count, ["Edit/Delete by user", "3"]);
                    menu.Add(menu.Count, ["Edit/Delete by hotel", "4"]);

                }
                menu.Add(menu.Count, ["Edit reservations", "5"]);
                if (user.IsAdmin)
                    menu.Add(menu.Count, ["Delete reservation", "6"]);

            }
            menu.Add(menu.Count, ["< Back", "999"]);
            menu.Add(menu.Count, ["Exit", "1000"]);

            return menu;
        }

        public static void ReservationsManagmentMenu(User? sUser, Reservation? sReservation)
        {
            bool running = true;
            
            if (user != null && sUser == null)
                sUser = user;

            Hotel? sHotel = null;

            while (running)
            {
                reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);

                Dictionary<int, string[]> menu = GetReservationsManagmentMenu();

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

                switch (option)
                {
                    case "0": // login/Logout
                        if (user == null)
                            user = UserHelper.UserLogin();
                        else
                        {
                            reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);
                            Console.Write("\tAre you sure to logout? (\"Y/n\"): ");
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                            {
                                user = null;
                                sUser = null;
                                sHotel = null;
                                sReservation = null;
                            }
                        }
                        if (user != null && !user.IsAdmin)
                            running = false;
                        break;
                    case "1": // View Reservations
                        if (user != null)
                        {
                            reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);
                            if (sHotel != null)
                                reservationHelper.PrintReservations((new List<Hotel> { sHotel }), true, true);
                            else if (sUser != null)
                                reservationHelper.PrintReservations((new List<User> { sUser }), true, true);
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "2": // View history of reservations
                        if (sUser != null || sHotel != null)
                        {
                            reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);
                            if (sHotel != null)
                                reservationHelper.PrintReservations((new List<Hotel> { sHotel }));
                            else if (sUser != null)
                                reservationHelper.PrintReservations((new List<User> { sUser }));
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "3": // Edit/Delete by user
                        sUser = UserHelper.SelectUser(user, sUser);
                        if (sUser != null)
                            sHotel = null;
                        break;
                    case "4": // Edit/Delete by hotel
                        sHotel = HotelHelper.SelectHotel(sHotel);
                        if (sHotel != null)
                            sUser = null;
                        break;
                    case "5": // Edit Reservations

                        if (sUser != null || sHotel != null)
                        {
                            reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);
                            if (sHotel != null)
                                sReservation = reservationHelper.SelectReservation(sHotel);
                            else if (sUser != null)
                                sReservation = reservationHelper.SelectReservation(sUser);

                            if ((sReservation != null) && reservationHelper.EditReservation(user, sUser, sHotel, sReservation))
                            {
                                Console.WriteLine($"\tReservation edited successfully.");
                                Console.WriteLine("\n\tPress any key to continue...");
                                Console.ReadKey();
                            }
                            sReservation = null;
                        }
                        break;
                    case "6": // Delete Reservation
                        
                        if (sUser != null || sHotel != null)
                        {
                            reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);
                            if (sHotel != null)
                                sReservation = reservationHelper.SelectReservation(sHotel);
                            else if (sUser != null)
                                sReservation = reservationHelper.SelectReservation(sUser);

                            if ((sReservation != null) && reservationHelper.DeleteReservation(sReservation))
                            {
                                Console.WriteLine($"\tReservation deleted successfully.");
                                Console.WriteLine("\n\tPress any key to continue...");
                                Console.ReadKey();
                                sReservation = null;
                            }
                            sReservation = null;
                        }
                        break;
                    case "999": // < Back
                        running = false;
                        Console.Clear();
                        break;
                    case "1000":
                        reservationHelper.PrintReservationsManagmentHeader(user, sUser, sHotel, sReservation);
                        Console.Write("\tExit application? (\"Y/n\"): ");
                        if ((Console.ReadLine() ?? "n").ToLower() != "y")
                            continue;
                        Environment.Exit(0);
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
