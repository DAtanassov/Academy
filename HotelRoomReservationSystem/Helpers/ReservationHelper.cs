using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class ReservationHelper
    {
        private static DataHelper dataHelper = new DataHelper();
        private static HotelHelper hotelHelper = new HotelHelper();

        public List<Reservation> GetUserReservations(int[] userId, bool current = false, bool onlyActive = false)
        {
            List<Reservation> reservations = DataHelper.GetReservationList();

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
            List<Reservation> reservations = DataHelper.GetReservationList();

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
            List<Reservation> reservations = DataHelper.GetReservationList();
            return reservations.Where(r => roomId.Contains(r.RoomId)).ToList();
        }

        public static void CheckAndCancelExpiredReservations()
        {
            List<Reservation> reservations = DataHelper.GetReservationList();
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

            DataHelper.UpdateReservations(reservations);

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

            List<Reservation> reservations = DataHelper.GetReservationList();
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
            List<Reservation> reservations = DataHelper.GetReservationList();

            Reservation reservation = new Reservation(user.Id, room, checkInDate, checkOutDate, RoomStatus.booked);
            DataHelper.InsertReservations([reservation]);

            PrintReservationInfo(reservation);

        }

        public static void PrintReservationInfo(Reservation reservation)
        {
            Hotel? hotel = HotelHelper.GetHotelById(reservation.HotelId);
            Room? room = RoomHelper.GetRoomById(reservation.RoomId, reservation.HotelId);

            Console.WriteLine($"\tReservation ID: {reservation.Id}");
            Console.WriteLine($"\tHotel: {((hotel == null) ? "<not found>" : hotel.Name)}");
            Console.WriteLine($"\tRoom: {((room == null) ? "" : room.ShortInfo())}");
            Console.WriteLine($"\tCheck-in date: {reservation.CheckInDate.ToShortDateString()}");
            Console.WriteLine($"\tCheck-out date: {reservation.CheckOutDate.ToShortDateString()}");
            Console.WriteLine($"\tTotal price: {reservation.TotalPrice}");
            Console.WriteLine($"\tStatus: {Enum.GetName(reservation.Status.GetType(), reservation.Status)}");
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
            Dictionary<string, string> rooms = RoomHelper.GetRooms(hotelId).ToDictionary(r => r.GetId(), r => r.ShortInfo());

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

            Dictionary<string, string> rooms = RoomHelper.GetRooms(hotelId).ToDictionary(r => r.GetId(), r => r.ShortInfo());

            foreach (Hotel hotel in hotels)
            {
                Console.WriteLine($"\n\t\t{hotel.Info()}:\n");
                List<Reservation> reservationByHotel = reservations.Where(r => r.HotelId == hotel.Id).OrderBy(r => r.RoomId).ToList();
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

            while (true)
            {
                Console.Clear();
                if (hotel != null)
                    Console.WriteLine($"\t\tReservations in {hotel.Info()}\n");
                else if (user != null)
                    Console.WriteLine($"\t\tReservations of user {user.Name}\n");

                // TODO - status menu admin/user
                int counter = 0;
                foreach (Reservation r in reservations)
                    Console.WriteLine($"\t{++counter}. {r.Info()}");

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

        public bool EditReservation(User? admin, User? user, Hotel? hotel, Reservation reservation)
        {
            List<Reservation> reservations = DataHelper.GetReservationList();

            int statusIndex;
            RoomStatus? status = null;
            
            // Edit status
            PrintReservationsManagmentHeader(admin, user, hotel, reservation);
            Console.Write("\tChange Status? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                var roomStatusType = typeof(RoomStatus);
                int[] roomStatusValues = (int[])roomStatusType.GetEnumValues();

                while (status == null)
                {
                    PrintReservationsManagmentHeader(admin, user, hotel, reservation);
                    Console.WriteLine("\tCurrent status: \"{0}\"", Enum.GetName(roomStatusType, reservation.Status));
                    
                    int counter = 0;
                    foreach (string s in Enum.GetNames(roomStatusType))
                        Console.WriteLine($"\t{counter++}. {s}");
                    Console.WriteLine($"\n\t{counter}. Cancel");

                    Console.Write("\tChoise new status: ");
                    if (!int.TryParse(Console.ReadLine() ?? "0", out statusIndex)
                        || !(roomStatusValues.Contains(statusIndex) || statusIndex == counter))
                    {

                        PrintReservationsManagmentHeader(admin, user, hotel, reservation);
                        Console.WriteLine("\tStatus not found");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return false;
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

            // TODO
            //Edit check-in and check-out dates
            //Edit room
            //Edit hotel
            //Edit user

            if ((reservation.Status == status))
            {
                PrintReservationsManagmentHeader(admin, user, hotel, reservation);
                Console.WriteLine("\tThe reservation not changed.");
                Console.WriteLine("\tPress any key to continue...");
                Console.ReadKey();
                return false;
            }

            reservation.Status = (RoomStatus)status;

            int index = reservations.FindIndex(r => r.Id == reservation.Id);
            if (index == -1)
                reservations.Add(reservation);
            else
                reservations[index] = reservation;

            DataHelper.UpdateReservations(reservations);

            return true;

        }

        public bool DeleteReservation(Reservation reservation)
        {
            List<Reservation> reservations = DataHelper.GetReservationList();
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

            reservations.RemoveAt(index);
            DataHelper.UpdateReservations(reservations);
            return true;
        }

        public void PrintReservationsManagmentHeader(User? admin, User? user, Hotel? hotel, Reservation? reservation)
        {
            Console.Clear();

            // Print first row
            if (reservation == null && user == null && hotel == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Reservation\t\t");

            if (admin == null)
                Console.WriteLine($"\t\t0. Login");
            else
                Console.WriteLine($"\t\tUser: \"{admin.Name}\" (0. Logout)");

            // Print second row
            if (user != null)
                Console.WriteLine($"\nFor user: \"{user.Name}\"");

            // Print third row
            if (reservation != null)
                Console.WriteLine("\n{0}", reservation.Info());
            else if (hotel != null)
                Console.WriteLine("\n{0}", hotel.ShortInfo());


            Console.WriteLine("\n\n");
        }
    }
}
