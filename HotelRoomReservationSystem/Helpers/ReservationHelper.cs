using HotelRoomReservationSystem.DB.JSON;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class ReservationHelper
    {
        private readonly static DBService<Reservation> reservationDBService = new DBService<Reservation>(new ReservationDB());

        public List<Reservation> GetUserReservations(int[] userId, bool current = false, bool onlyActive = false)
        {
            List<Reservation> reservations = reservationDBService.GetList();

            RoomStatus[] statuses = { RoomStatus.booked, RoomStatus.ocupated };

            if (current && onlyActive)
                return reservations.Where(r => userId.Contains(r.UserId) && DateTime.Now <= r.CheckOutDate && statuses.Contains(r.Status)).ToList();

            if (current && !onlyActive)
                return reservations.Where(r => userId.Contains(r.UserId) && DateTime.Now <= r.CheckOutDate ).ToList();

            if (!current && onlyActive)
                return reservations.Where(r => userId.Contains(r.UserId) && statuses.Contains(r.Status)).ToList();

            return reservations.Where(u => userId.Contains(u.UserId)).ToList();
        }

        public static List<Reservation> GetHotelReservations(int[] hotelId, bool current = false, bool onlyActive = false)
        {
            List<Reservation> reservations = reservationDBService.GetList();

            RoomStatus[] statuses = { RoomStatus.booked, RoomStatus.ocupated };

            if (current && onlyActive)
                return reservations.Where(r => hotelId.Contains(r.HotelId) && DateTime.Now <= r.CheckOutDate && statuses.Contains(r.Status)).ToList();

            if (current && !onlyActive)
                return reservations.Where(r => hotelId.Contains(r.HotelId) && DateTime.Now <= r.CheckOutDate).ToList();

            if (!current && onlyActive)
                return reservations.Where(r => hotelId.Contains(r.HotelId) && statuses.Contains(r.Status)).ToList();

            return reservations.Where(r => hotelId.Contains(r.HotelId)).ToList();
        }

        public List<Reservation> GetRoomReservations(int[] roomId)
        {
            List<Reservation> reservations = reservationDBService.GetList();
            return reservations.Where(r => roomId.Contains(r.RoomId)).ToList();
        }

        private Reservation? GetReservationById(List<Reservation> reservations, int id)
        {
            if (reservations.Count == 0)
                return null;
            return reservations.FirstOrDefault(r => r.Id == id);
        }

        public static void CheckAndCancelExpiredReservations()
        {
            List<Reservation> reservations = reservationDBService.GetList();
            if (reservations.Count == 0) return;
            
            DateTime toDate = DateTime.Today.Date.AddHours(12); // cancel reservation if not changet to "ocupated" befor 12h
            List<Reservation> booked = reservations.Where(r => (r.Status == RoomStatus.booked && r.CheckInDate < toDate)).ToList();
            if (booked.Count > 0)
            {
                foreach (Reservation reservation in booked)
                {
                    reservation.Status = RoomStatus.expired;
                    reservationDBService.Update(reservation);
                }
            }
        }

        public List<Room> SearchForAvailableRooms(Hotel? hotel = null, 
            DateTime? checkInDate = null, DateTime? checkOutDate = null, string location = "")
        {
            
            List<Hotel> hotels;
            if (hotel == null)
                hotels = HotelHelper.GetHotels();
            else
                hotels = new List<Hotel> { hotel };

            List<Room> rooms;
            if (!string.IsNullOrEmpty(location))
            {
                hotels = hotels.Where(h => h.Address.Contains(location)).ToList();
                rooms = RoomHelper.GetRooms(hotels.Select(h => h.Id).ToArray());
            }
            else if (hotel != null)
                rooms = RoomHelper.GetRooms([hotel.Id]);
            else
                rooms = RoomHelper.GetRooms();

            RoomStatus[] roomStatuses = { RoomStatus.booked, RoomStatus.ocupated };

            List<Reservation> reservations = reservationDBService.GetList();
            int[] roomsId = reservations.Where(r => (roomStatuses.Contains(r.Status) &&
                                                    ((r.CheckInDate <= checkInDate && checkInDate <= r.CheckOutDate)
                                                    || (r.CheckInDate <= checkOutDate && checkOutDate <= r.CheckOutDate)
                                                    || (checkInDate <= r.CheckInDate && r.CheckOutDate <= checkOutDate))))
                                        .Select(r => r.RoomId).ToArray();

            rooms = rooms.Where(r => !roomsId.Contains(r.Id)).ToList();

            return rooms;

        }

        public void BookTheRoom(Room room, User user, DateTime checkInDate, DateTime checkOutDate, string location)
        {
            List<Reservation> reservations = reservationDBService.GetList();

            Reservation reservation = new Reservation(user.Id, room, checkInDate, checkOutDate, RoomStatus.booked);
            reservationDBService.Insert(reservation);

            Console.WriteLine(reservation.Info());

        }

        public void PrintReservations(List<User> users, bool current = false, bool onlyActive = false)
        {
            List<Reservation> reservations = GetUserReservations(users.Select(u => u.Id).ToArray(), current, onlyActive);

            if (reservations.Count == 0)
            {
                Console.WriteLine("\n\tNo reservations found!");
                return;
            }
            
            int[] hotelId = reservations.Select(r => r.HotelId).Distinct().ToArray();

            Dictionary<int, string> hotels = HotelHelper.GetHotels(hotelId).ToDictionary(h => h.Id, h => h.Name);
            Dictionary<string, string> rooms = RoomHelper.GetRooms(hotelId).ToDictionary(r => r.Id.ToString(), r => r.ShortInfo());

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
                        string roomId = $"{reservation.RoomId}-{reservation.HotelId}";
                        if (rooms.TryGetValue(roomId, out string? value))
                            roomId = value;

                        Console.WriteLine($"\t\tRoom \"{roomId}\", " +
                            $"check-in \"{reservation.CheckInDate.ToShortDateString()}\", " +
                            $"check-out \"{reservation.CheckOutDate.ToShortDateString()}\",\n" +
                            $"\t\t\ttotal price \"{reservation.TotalPrice}\", " +
                            $"status \"{Enum.GetName(reservation.Status.GetType(), reservation.Status)}\"");
                    }
                }
            }
        }

        public void PrintReservations(List<Hotel> hotels, bool current = false, bool onlyActive = false)
        {
            int[] hotelId = hotels.Select(h => h.Id).ToArray();
            List<Reservation> reservations = GetHotelReservations(hotelId, current, onlyActive);

            if (reservations.Count == 0)
            {
                Console.WriteLine("\n\tNo reservations found!");
                return;
            }

            hotelId = reservations.Select(r => r.HotelId).Distinct().ToArray();

            Dictionary<int, string> rooms = RoomHelper.GetRooms(hotelId).ToDictionary(r => r.Id, r => r.ShortInfo());

            foreach (Hotel hotel in hotels)
            {
                Console.WriteLine($"\n\t\t{hotel.Info()}:\n");
                List<Reservation> reservationByHotel = reservations.Where(r => r.HotelId == hotel.Id).OrderBy(r => r.RoomId).ToList();
                foreach (Reservation reservation in reservationByHotel)
                {
                    string roomId = reservation.RoomId.ToString();
                    if (rooms.TryGetValue(reservation.RoomId, out string? value))
                        roomId = value;

                    Console.WriteLine($"\t\tRoom \"{roomId}\", " +
                        $"check-in \"{reservation.CheckInDate.ToShortDateString()}\", " +
                        $"check-out \"{reservation.CheckOutDate.ToShortDateString()}\",\n" +
                        $"\t\t\ttotal price \"{reservation.TotalPrice}\", " +
                        $"status \"{Enum.GetName(reservation.Status.GetType(), reservation.Status)}\"");
                }
            }
        }

        public Reservation? SelectReservation(User? user) => SelectReservation(user, null);

        public Reservation? SelectReservation(Hotel? hotel) => SelectReservation(null, hotel);

        public Reservation? SelectReservation(User? user, Hotel? hotel)
        {
            List<Reservation> reservations = new List<Reservation>();

            if (hotel != null)
                reservations = GetHotelReservations([hotel.Id]);
            else if (user != null)
                reservations = GetUserReservations([user.Id], true, true);

            if (reservations.Count == 0)
            {
                Console.WriteLine("\n\tNo reservations found!\n\n" +
                                    "\tPress any key to continue...");
                Console.ReadKey();
                return null;
            }

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            menuHelper.PrintAppName();

            if (hotel != null)
                Console.WriteLine($"\t\tReservations in {hotel.Info()}\n");
            else if (user != null)
                Console.WriteLine($"\t\tReservations of user {user.Name}\n");


            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            Func<string[], string[]> rName = (string[] n) => n;
            Dictionary<int, string[]> menu = reservations.Select((val, index) => new { Index = index, Value = val })
                                                    .ToDictionary(h => h.Index, h => rName([h.Value.Info(), h.Value.Id.ToString()]));
            menu.Add(menu.Count, ["Cancel", "0"]);
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
                            return GetReservationById(reservations, int.Parse(menu[menuParams.choice][1]));
                        running = false;
                        break;
                }

            }
            return null;
        }

        public bool EditReservation(User? admin, User? user, Hotel? hotel, Reservation reservation)
        {
            List<Reservation> reservations = reservationDBService.GetList();

            int statusIndex;
            var roomStatusType = typeof(RoomStatus);

            Room? room = RoomHelper.GetRoomById(statusIndex = reservation.RoomId, reservation.HotelId);
            User? resUser = (new UserHelper()).GetUserById(reservation.UserId);

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            ModelEnumHelper enumHelper = new ModelEnumHelper();
            menuHelper.PrintReservationsManagmentHeader(user, hotel, reservation);
            Console.WriteLine($"\t\tEdit reservation\n");

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            menuParams.choice = 8;
            bool cancel = false;
            bool running = true;

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Console.WriteLine($"\t{(menuParams.choice == 1 ? "• " : "  ")}1. ID:  {reservation.Id} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? "• " : "  ")}2. Hotel:  {(hotel == null ? "" : hotel.Name)} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? "• " : "  ")}3. Room:  {(room == null ? "" : room.ShortInfo())} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 4 ? "• " : "  ")}4. User:  {(resUser == null ? "" : resUser.Name)} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 5 ? "• " : "  ")}5. Check-in:  {reservation.CheckInDate.ToShortDateString()} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 6 ? "• " : "  ")}6. Check-out:  {reservation.CheckOutDate.ToShortDateString()} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 7 ? "• " : "  ")}7. Total prise:  {reservation.TotalPrice} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 8 ? menuParams.prefix : "  ")}8. Status:  {Enum.GetName(roomStatusType, reservation.Status)} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 9 ? menuParams.prefix : "  ")}9. Save\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 10 ? menuParams.prefix : "  ")}10. Cancel\u001b[0m");

                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        menuParams.choice = menuParams.choice == 8 ? 10 : menuParams.choice - 1;
                        continue;

                    case ConsoleKey.DownArrow:
                        menuParams.choice = menuParams.choice == 10 ? 8 : menuParams.choice + 1;
                        continue;

                    case ConsoleKey.Enter:
                        
                        switch (menuParams.choice)
                        {
                            case 8:
                                menuHelper.PrintReservationsManagmentHeader(user, hotel, reservation);
                                reservation.Status = ModelEnumHelper.SelectStatus(reservation.Status);
                                break;

                            case 9:
                                running = false;
                                break;
                            case 10:
                                cancel = true;
                                running = false;
                                break;

                        }

                        menuHelper.PrintReservationsManagmentHeader(user, hotel, reservation);
                        Console.WriteLine($"\t\tEdit reservation\n");
                        (menuParams.left, menuParams.top) = Console.GetCursorPosition();
                        break;
                }
            }

            if (cancel)
                return false;
            else
                reservationDBService.Update(reservation);

            return true;

        }

        public bool DeleteReservation(Reservation reservation)
        {
            List<Reservation> reservations = reservationDBService.GetList();
            int index = reservations.FindIndex(r => r.Id == reservation.Id && r.HotelId == reservation.HotelId);

            if (index == -1)
            {
                Console.WriteLine("\t Reservation not found. Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            Console.Clear();
            
            Console.WriteLine($"\t{reservation.Info()}");
            Console.Write($"\tDelete reservation? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            reservationDBService.Delete(reservation);
            return true;
        }

    }
}
