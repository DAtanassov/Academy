using System.Diagnostics.Metrics;
using HotelRoomReservationSystem.Helpers;
using static HotelRoomReservationSystem.Helpers.MenuHelper;

namespace HotelRoomReservationSystem.Models
{
    public class Menu
    {
        private static readonly MenuHelper menuHelper = new MenuHelper();
        private static readonly HotelHelper hotelHelper = new HotelHelper();

        public void Run()
        {
            FirstRun();
            MainMenu();
        }

        private void FirstRun()
        {
            bool adminNotRegistered = !UserHelper.isAdminRegistered();

            if (adminNotRegistered)
            {
                Console.CursorVisible = false;
                menuHelper.PrintAppName();
                Console.WriteLine("\n\t\tAdmin not found!\n\n");
                
                var menuParams = new MenuHelper.MenuParams();
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();

                while (adminNotRegistered)
                {
                    Console.SetCursorPosition(menuParams.left, menuParams.top);

                    Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Create Administrator\u001b[0m");
                    Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Exit\u001b[0m");

                    menuParams.key = Console.ReadKey(false);

                    switch (menuParams.key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            menuParams.choice = menuParams.choice == 1 ? 2 : menuParams.choice - 1;
                            break;

                        case ConsoleKey.DownArrow:
                            menuParams.choice = menuParams.choice == 2 ? 1 : menuParams.choice + 1;
                            break;

                        case ConsoleKey.Enter:
                            if (menuParams.choice == 1)
                                Program.user = UserHelper.AddUser(true);
                            else if (menuParams.choice == 2)
                            {
                                Console.Clear();
                                Environment.Exit(0);
                            }
                            break;
                    }

                    adminNotRegistered = !UserHelper.isAdminRegistered();
                }
                Console.CursorVisible = true;
            }
        }

        private void LoginLogoutMenu()
        {
            Console.CursorVisible = true;
            menuHelper.PrintAppName();

            if (Program.user == null)
                Program.user = UserHelper.UserLogin();
            else
            {
                Console.Write("\tAre you sure to logout? (\"Y/n\"): ");
                if ((Console.ReadLine() ?? "n").ToLower() == "y")
                    Program.user = null;
            }
            Console.CursorVisible = false;
        }

        private void ExitApplicationMenu()
        {
            menuHelper.PrintAppName();
            Console.CursorVisible = true;
            Console.Write("\tExit application? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                Console.Clear();
                Environment.Exit(0);
            }
        }

        private void MainMenu()
        {
            Console.CursorVisible = false;
            menuHelper.PrintAppName(); 
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            if (Program.user == null)
                menuParams.choice = 0;

            bool running = true;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Dictionary<int, string[]> menu = menuHelper.GetMainMenu(Program.user);

                menuHelper.PrintMenuElements(menu, menuParams);

                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 0 ? menu.Count - 1 : menuParams.choice - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == menu.Count - 1 ? 0 : menuParams.choice + 1;
                        break;

                    case ConsoleKey.Enter:
                        switch (menu[menuParams.choice][1])
                        {
                            case "0": // login/Logout
                                LoginLogoutMenu();
                                menuHelper.PrintAppName();
                                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                                break;
                            case "1": // Search
                                SearchMenu();
                                menuHelper.PrintAppName();
                                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                                break;
                            case "2": // User profile
                                      UserProfileMenu();
                                break;
                            case "3": // Users managment
                                if (Program.user != null)
                                    UsersManagmentMenu(null);
                                break;
                            case "4": // Hotels managment
                                if (Program.user != null)
                                    HotelManagmentMenu();
                                break;
                            case "5": // Reservations managment
                                ReservationsManagmentMenu(Program.user, null);
                                break;
                            case "1000": // Exit
                                ExitApplicationMenu();
                                break;
                        }
                        menuHelper.PrintAppName();
                        (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                        break;
                }
            }
        }
    
        private void SearchMenu()
        {
            ReservationHelper reservationHelper = new ReservationHelper();

            Hotel? hotel = null;
            string location = "";
            DateTime checkInDate = new DateTime();
            DateTime checkOutDate = new DateTime();

            Console.CursorVisible = false;
            menuHelper.PrintAppName();
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            bool running = true;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top); 
                
                menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location, false);
                
                Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Search in hotel\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Serch by location\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? menuParams.prefix : "  ")}3. < Back\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 4 ? menuParams.prefix : "  ")}4. Exit\u001b[0m");

                menuParams.key = Console.ReadKey(false);
                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 1 ? 4 : menuParams.choice - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == 4 ? 1 : menuParams.choice + 1;
                        break;

                    case ConsoleKey.Enter:

                        if (menuParams.choice == 1)
                        {
                            hotel = hotelHelper.SelectHotel();
                            running = false;
                            break;
                        }
                        else if (menuParams.choice == 2)
                        {
                            menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                            Console.Write("\tEnter location: ");
                            Console.CursorVisible = true;
                            location = Console.ReadLine() ?? string.Empty;
                            menuHelper.PrintAppName();
                            Console.CursorVisible = false;
                            running = false;
                            break;
                        }
                        else if (menuParams.choice == 3)
                            return;
                        else if (menuParams.choice == 4)
                        {
                            menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                            Console.Write("\tExit application? (\"Y/n\"): ");
                            Console.CursorVisible = true;
                            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                            {
                                Console.CursorVisible = false;
                                menuHelper.PrintAppName();
                                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                                continue;
                            }
                            Console.Clear();
                            Environment.Exit(0);
                        }
                        break;
                }
            }

            Console.CursorVisible = true;
            menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
            Console.Write("\tEnter check in date: ");
            while (!DateTime.TryParse(Console.ReadLine(), out checkInDate)
                || checkInDate <= DateTime.Today)
            {
                menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                Console.WriteLine("\tCheck-in date must be greater than current!");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                Console.Write("\tEnter check in date: ");
            }

            menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
            Console.Write("\tEnter check out date: ");
            while (!DateTime.TryParse(Console.ReadLine(), out checkOutDate)
                || checkOutDate <= DateTime.Today
                || checkOutDate < checkInDate)
            {
                menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                Console.WriteLine("\tCheck-out date must be greater than current and check-in date!");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                Console.Write("\tEnter check out date: ");
            }

            List<Room> rooms = reservationHelper.SearchForAvailableRooms(hotel, checkInDate, checkOutDate, location);

            List<Hotel> hotels;
            if (hotel == null)
                hotels = HotelHelper.GetHotels();
            else
                hotels = new List<Hotel> { hotel };

            Dictionary<int, Room> menu = new Dictionary<int, Room>();

            menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
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
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            menuParams.choice = 1;
            Console.CursorVisible = false;

            int counter = 0;
            Room? room = null;

            running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);
                counter = 0;
                menu.Clear();

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
                            menu.Add(++counter, r);
                            Console.WriteLine($"\t\t{(menuParams.choice == counter ? menuParams.prefix : "  ")}{counter}. {r.ShortInfo()}\u001b[0m");
                        }
                    }
                    counter++;
                    Console.WriteLine($"\t\t{(menuParams.choice == counter ? menuParams.prefix : "  ")}{counter}. Cancel\u001b[0m");
                }
                else
                {
                    rooms = rooms.OrderBy(o => o.HotelId).ThenBy(o => o.Number).ToList();
                    foreach (Room r in rooms)
                    {
                        menu.Add(++counter, r);
                        Console.WriteLine($"\t{(menuParams.choice == counter ? menuParams.prefix : "  ")}{counter}. {r.ShortInfo()}\u001b[0m");
                    }
                    counter++;
                    Console.WriteLine($"\t{(menuParams.choice == counter ? menuParams.prefix : "  ")}{counter}. Cancel\u001b[0m");
                }

                menuParams.key = Console.ReadKey(false);
                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 1 ? counter : menuParams.choice - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == counter ? 1 : menuParams.choice + 1;
                        break;

                    case ConsoleKey.Enter:

                        if (menuParams.choice == counter)
                        {
                            return;
                        }
                        else
                        {
                            room = menu[menuParams.choice];
                            running = false;
                        }
                        break;
                }
            }

            if (room == null)
                return;

            while (Program.user == null)
            {
                LoginLogoutMenu();
                menuHelper.PrintAppName();
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();

                if (Program.user == null)
                {
                    menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
                    Console.WriteLine("\tUser not logged in!");
                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to try again.");
                    ConsoleKeyInfo userInput = Console.ReadKey(true);
                    if (userInput.Key == ConsoleKey.Escape)
                        return;
                }

            } 

            menuHelper.PrintSearchMenuHeader(Program.user, hotel, checkInDate, checkOutDate, location);
            
            Console.WriteLine($"\tSelected room:\n{room.Info()}");

            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            menuParams.choice = 1;
            Console.CursorVisible = false;
            
            running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Book the room\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Search again\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? menuParams.prefix : "  ")}3. Cancel\u001b[0m");

                menuParams.key = Console.ReadKey(false);
                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 1 ? 3 : menuParams.choice - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == 3 ? 1 : menuParams.choice + 1;
                        break;

                    case ConsoleKey.Enter:
                        running = false;
                        break;
                }
            }

            if (menuParams.choice == 1)
            {
                reservationHelper.BookTheRoom(room, Program.user, checkInDate, checkOutDate, location);
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
            else if (menuParams.choice == 2)
                SearchMenu();

        }

        private void UserProfileMenu(Reservation? reservation = null)
        {
            Console.CursorVisible = false;
            menuHelper.PrintModelEditMenu(reservation);
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            if (Program.user == null)
                menuParams.choice = 0;

            string option = "";
            bool running = true;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Dictionary<int, string[]> menu = menuHelper.GetUserProfileMenu(Program.user);

                menuHelper.PrintMenuElements(menu, menuParams);

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
                        option = menu[menuParams.choice][1];
                        break;
                }

                switch (option)
                {
                    case "0": // login/Logout
                        LoginLogoutMenu();
                        if (Program.user != null && !Program.user.IsAdmin)
                            running = false;
                        break;
                    case "1": // View profile
                        if (Program.user != null)
                        {
                            menuHelper.PrintModelEditMenu(reservation);
                            Console.WriteLine(Program.user.Info());
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        break;
                    case "2": // Edit profile
                        if (Program.user != null && UserHelper.EditUser(Program.user, Program.user))
                        {
                            menuHelper.PrintModelEditMenu(reservation);
                            Console.WriteLine("\tProfile edited successfully!");
                            Program.user.Info();
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "3": // Delete profile
                        if (Program.user != null)
                        {
                            menuHelper.PrintModelEditMenu(reservation);
                            
                            string name = Program.user.Name;
                            UserHelper.DeleteUser(Program.user);
                            running = false;
                            Program.user = null;
                            reservation = null;
                            Console.WriteLine($"\tUser \"{name}\" delete successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "4": // Reservations menu
                        ReservationsManagmentMenu(Program.user, reservation);
                        break;
                    case "999":
                        running = false;
                        Console.Clear();
                        break;
                    case "1000":
                        ExitApplicationMenu();
                        break;
                }

                menuHelper.PrintModelEditMenu(reservation);
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            }
        }

        private void ReservationsManagmentMenu(User? sUser, Reservation? sReservation)
        {
            if (Program.user != null && sUser == null)
                sUser = Program.user;

            Hotel? sHotel = null;
            ReservationHelper reservationHelper = new ReservationHelper();
            string option = "";
            bool running = true;

            Console.CursorVisible = false;
            menuHelper.PrintReservationsManagmentHeader(sUser, sHotel, sReservation);
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            if (Program.user == null)
                menuParams.choice = 0;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Dictionary<int, string[]> menu = menuHelper.GetReservationsManagmentMenu(Program.user, sHotel);

                menuHelper.PrintMenuElements(menu, menuParams);

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
                        option = menu[menuParams.choice][1];
                        break;
                }

                switch (option)
                {
                    case "0": // login/Logout
                        LoginLogoutMenu();
                        if (Program.user == null)
                        { 
                                Program.user = null;
                                sUser = null;
                                sHotel = null;
                                sReservation = null;
                        }
                        if (Program.user != null && !Program.user.IsAdmin)
                            running = false;
                        break;
                    case "1": // View Reservations
                        if (Program.user != null)
                        {
                            menuHelper.PrintReservationsManagmentHeader(sUser, sHotel, sReservation);
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
                            menuHelper.PrintReservationsManagmentHeader(sUser, sHotel, sReservation);
                            if (sHotel != null)
                                reservationHelper.PrintReservations((new List<Hotel> { sHotel }));
                            else if (sUser != null)
                                reservationHelper.PrintReservations((new List<User> { sUser }));
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "3": // Edit/Delete by user
                        sUser = UserHelper.SelectUser(Program.user, sUser);
                        if (sUser != null)
                            sHotel = null;
                        break;
                    case "4": // Edit/Delete by hotel
                        sHotel = hotelHelper.SelectHotel(sHotel);
                        if (sHotel != null)
                            sUser = null;
                        break;
                    case "5": // Edit Reservations
                        if (sUser != null || sHotel != null)
                        {
                            menuHelper.PrintReservationsManagmentHeader(sUser, sHotel, sReservation);
                            if (sHotel != null)
                                sReservation = reservationHelper.SelectReservation(sHotel);
                            else if (sUser != null)
                                sReservation = reservationHelper.SelectReservation(sUser);

                            if ((sReservation != null) && reservationHelper.EditReservation(Program.user, sUser, sHotel, sReservation))
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
                            menuHelper.PrintReservationsManagmentHeader(sUser, sHotel, sReservation);
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
                        ExitApplicationMenu();
                        break;
                }

                menuHelper.PrintReservationsManagmentHeader(sUser, sHotel, sReservation);
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            }
        }

        private void UsersManagmentMenu(Reservation? reservation)
        {
            User? sUser = null;
            string option = "";
            bool running = true;

            Console.CursorVisible = false;
            menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            if (Program.user == null)
                menuParams.choice = 0;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Dictionary<int, string[]> menu = menuHelper.GetUsersManagmentMenu(Program.user, sUser);

                menuHelper.PrintMenuElements(menu, menuParams);

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
                        option = menu[menuParams.choice][1];
                        break;
                }

                switch (option)
                {
                    case "0": // login/Logout
                        LoginLogoutMenu();
                        break;
                    case "1": // View users
                        menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
                        UserHelper.PrintUsers(Program.user);
                        break;
                    case "2": // Add user
                        UserHelper.AddUser(false);
                        break;
                    case "3": // Edit user
                        if (Program.user != null && sUser == null)
                        {
                            menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
                            sUser = UserHelper.SelectUser(Program.user, sUser);
                        }
                        if (Program.user != null && sUser != null)
                        {
                            if (UserHelper.EditUser(Program.user, sUser))
                            {
                                menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
                                Console.WriteLine("\tProfile edited successfully!");
                                sUser.Info();
                                Console.WriteLine("\n\tPress any key to continue...");
                                Console.ReadKey();
                            }
                        }
                        break;
                    case "4": // Deselect user
                        sUser = null;
                        break;
                    case "5": // Edit another user
                        if (Program.user != null)
                        {
                            menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
                            sUser = UserHelper.SelectUser(Program.user, sUser);
                            if (sUser != null)
                            {
                                if (UserHelper.EditUser(Program.user, sUser))
                                {
                                    menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
                                    Console.WriteLine("\tProfile edited successfully!");
                                    sUser.Info();
                                    Console.WriteLine("\n\tPress any key to continue...");
                                    Console.ReadKey();
                                }
                            }
                        }
                        break;
                    case "6": // Delete user
                        if (sUser != null && Program.user != null && Program.user.Id != sUser.Id)
                        {
                            menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
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
                        ExitApplicationMenu();
                        break;
                }
                
                menuHelper.PrintUserManagmentHeader(Program.user, sUser, reservation);
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            }
        }

        private void HotelManagmentMenu()
        {
            Hotel? hotel = null;
            string option = "";
            bool running = true;

            Console.CursorVisible = false;
            menuHelper.PrintModelEditMenu(hotel);
            var menuParams = new MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            if (Program.user == null)
                menuParams.choice = 0;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);
                
                Dictionary<int, string[]> menu = menuHelper.GetHotelManagmentMenu(Program.user, hotel);

                menuHelper.PrintMenuElements(menu, menuParams);

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
                        option = menu[menuParams.choice][1];
                        break;
                }

                switch (option)
                {
                    case "0": // login/Logout
                        LoginLogoutMenu();
                        if (Program.user != null && !Program.user.IsAdmin)
                            running = false;
                        break;
                    case "1": // View hotels
                        hotelHelper.PrintHotels();
                        break;
                    case "2": // Add hotel
                        if (hotelHelper.AddHotel())
                        {

                        }
                        break;
                    case "3": // Edit hotel
                        menuHelper.PrintModelEditMenu(hotel);
                        hotel = hotelHelper.SelectHotel(null, Program.user);
                        if (hotel != null)
                            EditHotelMenu(hotel);
                        hotel = null;
                        break;
                    case "4": // Delete hotel
                        menuHelper.PrintModelEditMenu(hotel);
                        hotel = hotelHelper.SelectHotel();
                        if (hotel != null)
                        {
                            string hotelIndo = hotel.ShortInfo();
                            if (hotelHelper.DeleteHotel(hotel))
                            {
                                Console.WriteLine($"\t\"{hotelIndo}\" delete successfully.");
                                Console.WriteLine("\n\tPress any key to continue...");
                                Console.ReadKey();
                                hotel = null;
                            }
                        }
                        break;
                    case "999": // < Back
                        running = false;
                        Console.Clear();
                        break;
                    case "1000":
                        ExitApplicationMenu();
                        break;
                }

                menuHelper.PrintModelEditMenu(hotel);
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            }
        }

        private void EditHotelMenu(Hotel? hotel)
        {
            if (hotel == null)
                return;
            
            string option = "";
            bool running = true;

            Console.CursorVisible = false;
            menuHelper.PrintModelEditMenu(hotel);
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            Dictionary<int, string[]> menu = menuHelper.GetEditHotelMenu(true);

            while (running)
            {
                if (hotel == null)
                    return;
                
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
                        option = menu[menuParams.choice][1];
                        break;
                }

                switch (option)
                {
                    case "1": // Delete hotel
                        string hotelIndo = hotel.ShortInfo();
                        if (hotelHelper.DeleteHotel(hotel))
                        {
                            Console.WriteLine($"\t\"{hotelIndo}\" delete successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                            running = false;
                            hotel = null;
                        }
                        break;
                    case "2": // Edit hotel
                        if (Program.user != null && hotelHelper.EditHotel(hotel))
                        {
                            Console.WriteLine("\tHotel edited successfully!");
                            hotel.ShortInfo();
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "3": // Room managment
                        RoomManagmentMenu(hotel);
                        break;
                    case "999":
                        running = false;
                        break;
                    case "1000":
                        ExitApplicationMenu();
                        break;
                }

                menuHelper.PrintModelEditMenu(hotel);
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            }
        }

        private void RoomManagmentMenu(Hotel hotel)
        {
            Room? room;
            RoomType? roomType;
            RoomHelper roomHelper = new RoomHelper();
            RoomTypeHelper roomTypeHelper = new RoomTypeHelper();

            string option = "";
            bool running = true;

            Console.CursorVisible = false;
            menuHelper.PrintModelEditMenu(hotel);
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Dictionary<int, string[]> menu = menuHelper.GetHotelRoomManagmentMenu(true, hotel);

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
                        option = menu[menuParams.choice][1];
                        break;
                }

                switch (option)
                {

                    case "1": // Show rooms
                        RoomHelper.PrintRooms(hotel);
                        break;
                    case "2": // Show room types
                        RoomTypeHelper.PrintRoomTypes(hotel);
                        break;
                    case "3": // Add room
                        if (roomHelper.AddRoom(hotel))
                        {
                            Console.WriteLine("\tRoom edited successfully!");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "4": // Edit room
                        room = roomHelper.SelectRoom(hotel);
                        if (room != null && roomHelper.EditRoom(room))
                        {
                            Console.WriteLine("\tRoom edited successfully!");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        room = null;
                        break;
                    case "5": // Delete room
                        room = roomHelper.SelectRoom(hotel);
                        if (room != null && roomHelper.DeleteRoom(room))
                        {   
                            Console.WriteLine($"\tRoom {room.ShortInfo()} deleted successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        room = null;
                        break;
                    case "6": // Add room type
                        if (roomTypeHelper.AddRoomType(hotel))
                        {
                            Console.WriteLine("\tRoom type added successfully!");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "7": // Edit room type
                        roomType = roomTypeHelper.SelectRoomType(hotel);
                        if (roomType != null && roomTypeHelper.EditRoomType(roomType))
                        {
                            Console.WriteLine("\tRoom type edited successfully!");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "8": // Delete room type
                        roomType = roomTypeHelper.SelectRoomType(hotel);
                        if (roomType != null && roomTypeHelper.DeleteRoomType(roomType))
                        {   
                            Console.WriteLine($"\tRoom type {roomType.Name} deleted successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        roomType = null;
                        break;
                    case "999":
                        running = false;
                        break;
                    case "1000":
                        ExitApplicationMenu();
                        break;
                }

                menuHelper.PrintModelEditMenu(hotel);
                (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            }
        }

    }
}
