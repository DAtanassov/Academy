using System.Text.Json;
using System.Xml.Linq;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class ReservationHelper
    {
        private static DataHelper dataHelper = new DataHelper();
        private static HotelHelper hotelHelper = new HotelHelper();
        private static RoomHelper roomHelper = new RoomHelper();

        public List<Reservation> GetReservations()
        {
            string fileContent = dataHelper.GetFileContent("Reservations.json");

            List<Reservation>? reservations = new List<Reservation>();

            if (!String.IsNullOrEmpty(fileContent))
                reservations = JsonSerializer.Deserialize<List<Reservation>>(fileContent);

            if (reservations == null)
                return new List<Reservation>();

            return reservations;
        }

        public List<Reservation> GetUserReservations(int[] userId, bool current = false, bool onlyActive = false)
        {
            List<Reservation> reservations = GetReservations();

            RoomStatus[] statuses = { RoomStatus.booked, RoomStatus.ocupated };

            if (current && onlyActive)
                return reservations.Where(r => userId.Contains(r.UserId) && DateTime.Now <= r.CheckOutDate && statuses.Contains(r.Status)).ToList();

            if (current && !onlyActive)
                return reservations.Where(r => userId.Contains(r.UserId) && DateTime.Now <= r.CheckOutDate ).ToList();

            if (!current && onlyActive)
                return reservations.Where(r => userId.Contains(r.UserId) && statuses.Contains(r.Status)).ToList();

            return reservations.Where(u => userId.Contains(u.UserId)).ToList();
        }

        public List<Reservation> GetHotelReservations(int[] hotelId)
        {
            List<Reservation> reservations = GetReservations();
            return reservations.Where(h => hotelId.Contains(h.HotelId)).ToList();
        }

        public List<Reservation> GetRoomReservations(int[] roomId)
        {
            List<Reservation> reservations = GetReservations();
            return reservations.Where(r => roomId.Contains(r.RoomId)).ToList();
        }

        public void CheckAndCancelExpiredReservations()
        {
            List<Reservation> reservations = GetReservations();
            if (reservations.Count == 0) return;
            
            DateTime toDate = DateTime.Today.Date.AddHours(12); // cancel reservation if not changet to "ocupated" befor 12h
            List<Reservation> booked = reservations.Where(r => (r.Status == RoomStatus.booked && r.CheckInDate < toDate)).ToList();
            if (booked.Count > 0)
            {
                foreach (Reservation reservation in booked)
                {
                    reservation.Status = RoomStatus.expired;
                    int rIndex = reservations.FindIndex(r => r.Id == reservation.Id);
                    reservations[rIndex] = reservation;
                }
            }

            dataHelper.WriteUpdateReservations(reservations);

        }

        private static void PrintSearchHeader(Hotel? hotel, User? user,
            string? checkInDate = null, string? checkOutDate = null, string? location = null)
        {
            Console.Clear();

            // Print first row (Hotel, User)
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
            // TODO = show only date
            Console.WriteLine("\n");
            Console.Write($"Check-in: {((checkInDate == null) ? "<not selected>" : checkInDate)}");
            Console.Write($"\tCheck-out: {((checkOutDate == null) ? "<not selected>" : checkOutDate)}");

            Console.WriteLine("\n\n");
        }

        public void SearchForAvailableRooms(Hotel? hotel, User? user)
        {
            DateTime checkInDate = new DateTime();
            DateTime checkOutDate = new DateTime();
            string? location = "";

            PrintSearchHeader(hotel, user);
            Console.Write("Enter check in date: ");
            while (!DateTime.TryParse(Console.ReadLine(), out checkInDate)
                || checkInDate <= DateTime.Today)
            {
                PrintSearchHeader(hotel, user);
                Console.WriteLine("\tCheck-in date must be greater than current!");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                PrintSearchHeader(hotel, user);
                Console.Write("Enter check in date: ");
            }

            PrintSearchHeader(hotel, user, checkInDate.ToShortDateString());
            Console.Write("Enter check out date: ");
            while (!DateTime.TryParse(Console.ReadLine(), out checkOutDate)
                || checkOutDate <= DateTime.Today
                || checkOutDate < checkInDate)
            {
                PrintSearchHeader(hotel, user, checkInDate.ToShortDateString());
                Console.WriteLine("\tCheck-out date must be greater than current and check-in date!");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                PrintSearchHeader(hotel, user, checkInDate.ToShortDateString());
                Console.Write("Enter check out date: ");
            }

            if (hotel == null)
            {
                PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString());
                Console.Write("Enter location: ");
                location = Console.ReadLine() ?? string.Empty;
            }

            List<Hotel> hotels;
            if (hotel == null)
                hotels = hotelHelper.GetHotels();
            else
                hotels = new List<Hotel> { hotel };

            List<Room> rooms = new List<Room>();

            if (!string.IsNullOrEmpty(location))
            {
                hotels = hotels.Where(h => h.Address.Contains(location)).ToList();
                rooms = roomHelper.GetRooms(hotels.Select(h => h.Id).ToArray());
            }
            else if (hotel != null)
                rooms = roomHelper.GetRooms([hotel.Id]);
            else
                rooms = roomHelper.GetRooms(null);

            RoomStatus[] roomStatuses = { RoomStatus.booked, RoomStatus.ocupated };

            List<Reservation> reservations = GetReservations();
            int[] roomsId = reservations.Where(r => (roomStatuses.Contains(r.Status) &&
                                                    ((r.CheckInDate <= checkInDate && checkInDate <= r.CheckOutDate)
                                                    || (r.CheckInDate <= checkOutDate && checkOutDate <= r.CheckOutDate)
                                                    || (checkInDate <= r.CheckInDate && r.CheckOutDate <= checkOutDate))))
                                        .Select(r => r.RoomId).ToArray();

            rooms = rooms.Where(r => !roomsId.Contains(r.Id)).ToList();

            Dictionary<int, Room> menu = new Dictionary<int, Room>();

            PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString(), location);
            if (rooms.Count > 0)
                Console.WriteLine("\n\tAvailable rooms:\n");
            int counter = 0;
            if (hotel == null)
            {
                int[] hotelsId = rooms.Select(r => r.HotelId).ToArray();
                hotels = hotels.Where(h => hotelsId.Contains(h.Id)).OrderBy(o => o.Name).ToList();
                foreach (Hotel h in hotels)
                {
                    Console.WriteLine($"\tHotel \"{h.Name}\"");

                    List<Room> hRooms = rooms.Where(r => r.HotelId == h.Id).OrderBy(o => o.Number).ToList();
                    foreach (Room r in hRooms)
                    {
                        Console.WriteLine($"\t\t{++counter}. {r.RoomPresentation()}");
                        menu.Add(counter, r);
                    }
                }
            }
            else
            {
                rooms = rooms.OrderBy(o => o.HotelId).ThenBy(o => o.Number).ToList();
                foreach (Room r in rooms)
                {
                    Console.WriteLine($"\t{++counter}. {r.RoomPresentation()}");
                    menu.Add(counter, r);
                }

            }

            if (rooms.Count == 0)
            {
                Console.WriteLine("\n\t Rooms not found");
                Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to search again.");
                ConsoleKeyInfo userInput = Console.ReadKey(true);
                if (userInput.Key == ConsoleKey.Escape)
                    return;
                else if (!Char.IsNumber(userInput.KeyChar))
                    SearchForAvailableRooms(hotel, user);
                else
                {
                    Int32 llComand;
                    if (Int32.TryParse(userInput.KeyChar.ToString(), out llComand))
                        if (llComand == 0)
                        {
                            if (user == null)
                            {
                                UserHelper userHelper = new UserHelper();
                                Console.Clear();
                                Console.Write("\n\tUsername: ");
                                string username = Console.ReadLine() ?? "";
                                Console.Clear();
                                Console.Write("\n\tPassword: ");
                                string password = Console.ReadLine() ?? "";
                                user = userHelper.GetUser(username, password);
                            }
                            else
                                user = null;
                        }
                    SearchForAvailableRooms(hotel, user);
                }
            }

            Console.WriteLine($"\t{++counter}. Cancel");

            Console.Write("\n\n\tChoose a room: ");
            int choice;
            if (!int.TryParse((Console.ReadLine() ?? "-1"), out choice)
                || !(menu.ContainsKey(choice) || choice == counter))
            {
                PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString(), location);
                Console.WriteLine("\nInvalid option. Press any key to try again.");
                Console.ReadKey();
                return;
            }

            if (choice == counter)
                return;

            while(user == null)
            {
                PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString(), location);

                UserHelper userHelper = new UserHelper();
                Console.Clear();
                Console.Write("\n\tUsername: ");
                string username = Console.ReadLine() ?? "";
                Console.Clear();
                Console.Write("\n\tPassword: ");
                string password = Console.ReadLine() ?? "";
                user = userHelper.GetUser(username, password);
                if (user == null)
                {
                    PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString(), location);
                    Console.WriteLine("\tUser not logged in!");
                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to try again.");
                    ConsoleKeyInfo userInput = Console.ReadKey(true);
                    if (userInput.Key == ConsoleKey.Escape)
                        return;
                }
                
            }

            Room room = menu[choice];

            BookTheRoom(hotel, room, user, checkInDate, checkOutDate, location);

        }

        public void BookTheRoom(Hotel? hotel, Room room, User? user, DateTime checkInDate, DateTime checkOutDate, string location)
        {
            PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString(), location);

            Console.WriteLine($"\tSelected room:\n" +
                              $"{room.RoomInfo()}");

            Console.WriteLine($"\n\t1. Book the room\n" +
                                $"\t2. Search again\n" +
                                $"\t3. Cancel");

            Console.Write("\n\n\tChoose an option: ");
            int choice = 0;
            while (!int.TryParse((Console.ReadLine() ?? "0"), out choice)
                || !(choice >= 1 && choice <= 3))
            {
                PrintSearchHeader(hotel, user, checkInDate.ToShortDateString(), checkOutDate.ToShortDateString(), location);

                Console.WriteLine("\nInvalid option. Press any key to try again.");
                Console.ReadKey();
                BookTheRoom(hotel, room, user, checkInDate, checkOutDate, location);
                return;
            }

            switch (choice)
            {
                case 1:
                    
                    if (user == null) return;

                    List<Reservation> reservations = GetReservations();

                    Reservation reservation = new Reservation(user.Id, room, checkInDate, checkOutDate, RoomStatus.booked);
                    reservation.Id = GetMaxReservationId(reservations);
                    reservations.Add(reservation);


                    dataHelper.WriteUpdateReservations(reservations);

                    PrintReservationInfo(reservation);

                    break;
                case 2:
                    SearchForAvailableRooms(hotel, user);
                    return;
                default:
                    return;
            }
        }

        public static void PrintReservationInfo(Reservation reservation)
        {
            Hotel? hotel = hotelHelper.GetHotelById(reservation.HotelId);
            Room? room = roomHelper.GetRoomById(reservation.RoomId);

            Console.WriteLine($"\tReservation ID: {reservation.Id}");
            Console.WriteLine($"\tHotel: {((hotel == null) ? "<not found>" : hotel.Name)}");
            Console.WriteLine($"\tRoom: {((room == null) ? "" : room.RoomPresentation())}");
            Console.WriteLine($"\tCheck-in date: {reservation.CheckInDate.ToString("dd.MM.yyy")}");
            Console.WriteLine($"\tCheck-out date: {reservation.CheckOutDate.ToString("dd.MM.yyy")}");
            Console.WriteLine($"\tTotal price: {reservation.TotalPrice}");
            Console.WriteLine($"\tStatus: {Enum.GetName(reservation.Status.GetType(), reservation.Status)}");
        }

        public void PrintUsersReservations(List<User> users, bool current = false, bool onlyActive = false)
        {
            List<Reservation> reservations = GetUserReservations(users.Select(u => u.Id).ToArray(), current, onlyActive);

            if (reservations.Count == 0)
            {
                Console.WriteLine("\n\tNo reservations found!");
                return;
            }
                

            int[] hotelId = reservations.Select(r => r.HotelId).ToArray();

            Dictionary<int, string> hotels = hotelHelper.GetHotels(hotelId).ToDictionary(h => h.Id, h => h.Name);
            Dictionary<int, string> rooms = roomHelper.GetRooms(hotelId).ToDictionary(r => r.Id, r => r.RoomPresentation());

            foreach (User user in users)
            {
                Console.WriteLine($"\tUser \"{user.Name}\" reservations:");
                List<Reservation> userReservations = reservations.Where(r => r.UserId == user.Id).ToList();

                SortedSet<int> hotelsHash = new SortedSet<int>(hotelId);
                foreach (int h in hotelsHash)
                {
                    Console.WriteLine($"\n\t\tHotel \"{hotels[h]}\":\n");
                    List<Reservation> reservationByHotel = userReservations.Where(r => r.HotelId == h).OrderBy(r => r.RoomId).ToList();
                    foreach (Reservation reservation in reservationByHotel)
                    {
                        Console.WriteLine($"\t\tRoom \"{rooms[reservation.RoomId]}\", " +
                            $"check-in \"{reservation.CheckInDate.ToString("dd.MM.yyy")}\", " +
                            $"check-out \"{reservation.CheckOutDate.ToString("dd.MM.yyy")}\",\n" +
                            $"\t\t\ttotal price \"{reservation.TotalPrice}\", " +
                            $"status \"{Enum.GetName(reservation.Status.GetType(), reservation.Status)}\"");
                    }
                }
            }
        }

        private Reservation? SelectReservation(User? user)
        {
            if (user == null)
                return null;

            List<Reservation> reservations = GetUserReservations([ user.Id ], true, true);

            if (reservations.Count == 0)
            {
                Console.WriteLine("\n\tNo reservations found!\n\n" +
                                    "\tPress any key to continue...");
                Console.ReadKey();
                return null;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t\tReservations of user {user.Name}\n");

                // TODO - status menu admin/user
                int counter = 0;
                foreach (Reservation r in reservations)
                    Console.WriteLine($"\t{++counter}. {r.GetShortInfo()}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a reservation: ");

                int choice;
                Console.Write("\t");
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= reservations.Count)
                        return reservations[choice - 1];
                    else if (choice == counter)
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\n\tInvalid option.Press \"Esc\" for cancel or any other key to try again.");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return null;

                    }
                }
            }

            return null;
        }

        private void EditReservation(User? admin, User? user, Reservation reservation)
        {
            List<Reservation> reservations = GetReservations();

            int statusIndex;
            RoomStatus? status = null;
            Hotel? hotel = null;
            Room? room = null;
            User? newUser = null;
            DateTime? checkInDate = null;
            DateTime? checkOutDate = null;

            // Edit status
            PrintReservationsManagmentHeader(admin, user, reservation);
            Console.Write("\tChange Status? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                var roomStatusType = typeof(RoomStatus);
                int[] roomStatusValues = (int[])roomStatusType.GetEnumValues();

                while (status == null)
                {
                    PrintReservationsManagmentHeader(admin, user, reservation);
                    Console.WriteLine("\tCurrent status: \"{0}\"", Enum.GetName(roomStatusType, reservation.Status));
                    
                    int counter = 0;
                    foreach (string s in Enum.GetNames(roomStatusType))
                        Console.WriteLine($"\t{counter++}. {s}");
                    Console.WriteLine($"\n\t{counter}. Cancel");

                    Console.Write("Choise new status: ");
                    if (!int.TryParse(Console.ReadLine() ?? "0", out statusIndex)
                        || !(roomStatusValues.Contains(statusIndex) || statusIndex == counter))
                    {

                        PrintReservationsManagmentHeader(admin, user, reservation);
                        Console.WriteLine("\tStatus not found");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return;
                    }
                    else
                    {
                        if (statusIndex == counter)
                            status = reservation.Status;
                        else
                            status = (RoomStatus)statusIndex;
                    }
                }
            }
            else
                status = reservation.Status;

            //Edit check-in and check-out dates
            //Edit room
            //Edit hotel
            //Edit user

            reservation.Status = (RoomStatus)status;

            int index = reservations.FindIndex(r => r.Id == reservation.Id);
            if (index == -1)
                reservations.Add(reservation);
            else
                reservations[index] = reservation;

            dataHelper.WriteUpdateReservations(reservations);

        }

        private Dictionary<int, string[]> GetReservationManagmentMenu(User? admin, User? user, Reservation? reservation)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count, ["Login/Logout", "0"]);

            if (user != null)
            {
                menu.Add(menu.Count, ["View Reservations", "1"]);
                if (reservation == null)
                    menu.Add(menu.Count, ["Select Reservation", "2"]);
                else
                {
                    menu.Add(menu.Count, ["Deselect Reservation", "3"]);
                    menu.Add(menu.Count, ["Edit Reservation", "4"]);
                    if (admin != null && admin.IsAdmin)
                        menu.Add(menu.Count, ["Delete Reservation", "5"]);
                }
                menu.Add(menu.Count, ["View history of reservations", "6"]);
            }
            menu.Add(menu.Count, ["< Back", "1000"]);

            return menu;
        }

        public void ReservationManagmentMenu(User? admin, User? user, Reservation? reservation)
        {
            bool running = true;

            if (user == null && admin != null && !admin.IsAdmin)
                user = admin;

            while (running)
            {
                PrintReservationsManagmentHeader(admin, user, reservation);

                Dictionary<int, string[]> menu = GetReservationManagmentMenu(admin, user, reservation);

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
                        break;
                    case "1": // View Reservations
                        if (user != null)
                        {
                            PrintReservationsManagmentHeader(admin, user, reservation);
                            PrintUsersReservations(new List<User> { user }, true, true);
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "2": // Select Reservation
                        if (user != null)
                        {
                            PrintReservationsManagmentHeader(admin, user, reservation);
                            reservation = SelectReservation(user);
                        }
                        break;
                    case "3": // Deselect Reservation
                        reservation = null;
                        break;
                    case "4": // Edit Reservation
                        if (reservation != null)
                            EditReservation(admin, user, reservation);
                        break;
                    case "5": // Delete Reservation
                        
                        break;
                    case "6": // View history of reservations
                        if (user != null)
                        {
                            PrintReservationsManagmentHeader(admin, user, reservation);
                            PrintUsersReservations(new List<User> { user });
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
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

        private static void PrintReservationsManagmentHeader(User? admin, User? user, Reservation? reservation)
        {
            Console.Clear();

            // Print first row
            if (reservation == null && user == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Reservations\t\t");

            if (admin == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{admin.Name}\" (0. Logout)");

            // Print second row
            if (user != null)
                Console.Write($"\nUser: \"{user.Name}\"");
            // Print third row
            if (reservation != null)
                Console.Write($"\n{reservation.GetShortInfo()}");

            Console.WriteLine("\n\n");
        }

        private int GetMaxReservationId(List<Reservation> reservations)
        {
            if (reservations.Count == 0)
                return 1;

            int maxId = reservations.Max(r => r.Id);
            return ++maxId;
        }
    }
}
