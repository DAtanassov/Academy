using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class MenuHelper
    {
        public static readonly string appName = "Hotel room reservation system";
        public struct MenuParams
        {
            public int left { get; set; }
            public int top { get; set; }
            public int choice { get; set; } = 1;
            public string prefix { get; } = "\u001b[32m• ";
            public ConsoleKeyInfo key { get; set; }
            public MenuParams() { }
        }

        public void PrintAppName(bool clear = true)
        {
            if (clear)
                Console.Clear();
            Console.WriteLine($"\t\t\u001b[1m{appName}\u001b[0m\n");
        }

        public void PrintMenuElements(Dictionary<int, string[]> menu, MenuParams menuParams, bool printLogin = true)
        {
            foreach (var element in menu)
            {
                Console.WriteLine($"\t{((element.Key == 0 && printLogin) ? "\t\t\t\t" : "")}" +
                                    $"{(menuParams.choice == element.Key ? menuParams.prefix : "  ")}" +
                                    $"{element.Key}. {element.Value[0]}\u001b[0m");
            }

        }

        public Dictionary<int, string[]> GetMainMenu(User? user)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (user == null)
                menu.Add(menu.Count, ["\u001b[1mLogin\u001b[0m", "0"]);
            else
                menu.Add(menu.Count, [$"\u001b[1m\"{user.Name}\" (Logout)\u001b[0m", "0"]);

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

        public void PrintSearchMenuHeader(User? user, Hotel? hotel, DateTime? checkInDate, DateTime? checkOutDate, string? location, bool printName = true)
        {
            if (printName)
                PrintAppName();

            // Print first row (Hotel/location, User)
            if (hotel == null)
                if (!string.IsNullOrEmpty(location))
                    Console.Write($"Search in location: {location.Trim()}");
                else
                    Console.Write($"Search in all hotels");
            else
                Console.Write($"Search in hotel: \"{hotel.Name}\"");

            if (user != null)
                Console.Write($"\t\tUser: \"{user.Name}\"");

            // Print Second row check-in/out
            Console.WriteLine("\n");
            Console.Write($"Check-in: {((checkInDate == null) ? "<not selected>" : ((DateTime)checkInDate).ToShortDateString())}");
            Console.Write($"\tCheck-out: {((checkOutDate == null) ? "<not selected>" : ((DateTime)checkOutDate).ToShortDateString())}");

            Console.WriteLine("\n");
        }

        public Dictionary<int, string[]> GetUserProfileMenu(User? user)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (user == null)
                menu.Add(menu.Count, ["\u001b[1mLogin\u001b[0m", "0"]);
            else
                menu.Add(menu.Count, [$"\u001b[1m\"{user.Name}\" (Logout)\u001b[0m", "0"]);

            if (Program.user != null)
            {
                menu.Add(menu.Count, ["View profile", "1"]);
                menu.Add(menu.Count, ["Edit profile", "2"]);
                if (!Program.user.IsAdmin)
                    menu.Add(menu.Count, ["Delete profile", "3"]);
                menu.Add(menu.Count, ["Reservations menu", "4"]);
            }
            menu.Add(menu.Count, ["< Back", "999"]);
            menu.Add(menu.Count, ["Exit", "1000"]);

            return menu;
        }

        public Dictionary<int, string[]> GetReservationsManagmentMenu(User? user, Hotel? hotel)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (user == null)
                menu.Add(menu.Count, ["\u001b[1mLogin\u001b[0m", "0"]);
            else
                menu.Add(menu.Count, [$"\u001b[1m\"{user.Name}\" (Logout)\u001b[0m", "0"]);

            bool hotelManager = (user != null && HotelHelper.UserIsHotelManager(user.Id, (hotel == null) ? 0 : hotel.Id));
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

        public void PrintReservationsManagmentHeader(User? user, Hotel? hotel, Reservation? reservation, bool printName = true)
        {
            if (printName)
                PrintAppName();

            // Print first row
            if (!(reservation == null && user == null && hotel == null))
                Console.Write($"Reservation\t\t");

            // Print second row
            if (user != null)
                Console.WriteLine($"\nFor user: \"{user.Name}\"");

            // Print third row
            if (reservation != null)
                Console.WriteLine("\n{0}", reservation.Info());
            else if (hotel != null)
                Console.WriteLine("\n{0}", hotel.ShortInfo());

            if (!(reservation == null && user == null && hotel == null))
                Console.WriteLine("\n");
        }

        public Dictionary<int, string[]> GetUsersManagmentMenu(User? admin, User? user)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (admin == null)
                menu.Add(menu.Count, ["\u001b[1mLogin\u001b[0m", "0"]);
            else
                menu.Add(menu.Count, [$"\u001b[1m\"{admin.Name}\" (Logout)\u001b[0m", "0"]);

            if (admin != null)
            {
                if (admin.IsAdmin || admin.CompareTo(user) == 0)
                {
                    menu.Add(menu.Count, ["View users", "1"]);
                    menu.Add(menu.Count, ["Add user", "2"]);

                    if (user == null)
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

        public void PrintUserManagmentHeader(User? admin, User? user, Reservation? reservation, bool printName = true)
        {
            if (printName)
                PrintAppName();

            // Print first row (Hotel, User)
            if (!(reservation == null && user == null))
                Console.Write($"Edit:");

            // Print second row
            if (user != null)
                Console.Write($"\nUser: \"{user.Name}\"");
            if (reservation != null)
                Console.Write($"\nReservation: \"{reservation.Id.ToString()}\"");

            if (!(reservation == null && user == null))
                Console.WriteLine("\n");
        }

        public Dictionary<int, string[]> GetHotelManagmentMenu(User? user, Hotel? hotel)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (user == null)
                menu.Add(menu.Count, ["\u001b[1mLogin\u001b[0m", "0"]);
            else
                menu.Add(menu.Count, [$"\u001b[1m\"{user.Name}\" (Logout)\u001b[0m", "0"]);

            menu.Add(menu.Count, ["View hotels", "1"]);

            bool hotelManager = (user != null && HotelHelper.UserIsHotelManager(user.Id, (hotel == null) ? 0 : hotel.Id));
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

        public Dictionary<int, string[]> GetEditHotelMenu(bool isAdmin = false)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (isAdmin)
            {
                menu.Add(menu.Count + 1, ["Delete hotel", "1"]);
                menu.Add(menu.Count + 1, ["Edit hotel", "2"]);

            }
            menu.Add(menu.Count + 1, ["Room managment", "3"]);
            menu.Add(menu.Count + 1, ["< Back", "999"]);
            menu.Add(menu.Count + 1, ["Exit", "1000"]);
            return menu;
        }

        public Dictionary<int, string[]> GetHotelRoomManagmentMenu(bool isAdmin, Hotel hotel)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            List<RoomType> roomTypes = RoomTypeHelper.GetRoomTypes([hotel.Id]);

            menu.Add(menu.Count + 1, ["Show rooms", "1"]);
            menu.Add(menu.Count + 1, ["Show room types", "2"]);
            if (isAdmin)
            {
                if (roomTypes.Count > 0)
                {
                    menu.Add(menu.Count + 1, ["Add room", "3"]);
                    menu.Add(menu.Count + 1, ["Edit room", "4"]);
                    menu.Add(menu.Count + 1, ["Delete room", "5"]);
                }

                menu.Add(menu.Count + 1, ["Add room type", "6"]);
                menu.Add(menu.Count + 1, ["Edit room type", "7"]);
                menu.Add(menu.Count + 1, ["Delete room type", "8"]);

            }
            menu.Add(menu.Count + 1, ["< Back", "999"]);
            menu.Add(menu.Count + 1, ["Exit", "1000"]);

            return menu;
        }

        public void PrintModelEditMenu<T>(T? item, bool printName = true) where T : BaseModel
        {
            if (printName)
                PrintAppName();

            // Print first row (T)
            if (item != null)
                Console.WriteLine($"Edit:\n{item.ShortInfo()}\n");
        }

        public void PrintAddUserHeader(string additionalText = "", bool printName = true)
        {
            if (printName)
                PrintAppName();

            Console.WriteLine("\t\tAdd user\n");
            if (!string.IsNullOrEmpty(additionalText))
                Console.Write(additionalText);
        }

        public Dictionary<int, string[]> GetAmenitiesMenu(List<string> amenities)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            foreach (string amenity in amenities)
                menu.Add(menu.Count + 1, [amenity]);
            menu.Add(menu.Count + 1, ["Delete all amenities"]);
            menu.Add(menu.Count + 1, ["Save"]);
            menu.Add(menu.Count + 1, ["Cancel"]);

            return menu;
        }

        public void PrintUserManagmentHeader(User? user, Reservation? reservation, bool printName = true)
        {
            if (printName)
                PrintAppName();

            // Print first row (User)
            if (reservation == null && user == null)
                Console.Write($"Edit:");

            // Print second row
            if (user != null)
                Console.Write($"\nUser: \"{user.Name}\"");
            if (reservation != null)
                Console.Write($"\nReservation: \"{reservation.Id.ToString()}\"");

            if (reservation == null && user == null) 
                Console.WriteLine("\n");
        }

    }
}
