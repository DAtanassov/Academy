using HotelRoomReservationSystem.Models;
using System.Text.Json;

namespace HotelRoomReservationSystem.Helpers
{
    public class ReservationHelper
    {
        private static DataHelper dataHelper = new DataHelper();
        private static HotelHelper hotelHelper = new HotelHelper();

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

        public List<Room> SearchForAvailableRooms(Hotel? hotel = null, 
            DateTime? checkInDate = null, DateTime? checkOutDate = null, string location = "")
        {
            
            List<Hotel> hotels;
            if (hotel == null)
                hotels = hotelHelper.GetHotels();
            else
                hotels = new List<Hotel> { hotel };

            List<Room> rooms;
            if (!string.IsNullOrEmpty(location))
            {
                hotels = hotels.Where(h => h.Address.Contains(location)).ToList();
                rooms = hotelHelper.GetRooms(hotels.Select(h => h.Id).ToArray());
            }
            else if (hotel != null)
                rooms = hotelHelper.GetRooms([hotel.Id]);
            else
                rooms = hotelHelper.GetRooms();

            RoomStatus[] roomStatuses = { RoomStatus.booked, RoomStatus.ocupated };

            List<Reservation> reservations = GetReservations();
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
            List<Reservation> reservations = GetReservations();

            Reservation reservation = new Reservation(user.Id, room, checkInDate, checkOutDate, RoomStatus.booked);
            reservation.Id = GetMaxId(reservations, room.HotelId);

            reservations.Add(reservation);
            dataHelper.WriteUpdateReservations(reservations);

            PrintReservationInfo(reservation);

        }

        public static void PrintReservationInfo(Reservation reservation)
        {
            Hotel? hotel = hotelHelper.GetHotelById(reservation.HotelId);
            Room? room = hotelHelper.GetRoomById(reservation.RoomId, reservation.HotelId);

            Console.WriteLine($"\tReservation ID: {reservation.Id}");
            Console.WriteLine($"\tHotel: {((hotel == null) ? "<not found>" : hotel.Name)}");
            Console.WriteLine($"\tRoom: {((room == null) ? "" : room.RoomPresentation())}");
            Console.WriteLine($"\tCheck-in date: {reservation.CheckInDate.ToShortDateString()}");
            Console.WriteLine($"\tCheck-out date: {reservation.CheckOutDate.ToShortDateString()}");
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
            Dictionary<int, string> rooms = hotelHelper.GetRooms(hotelId).ToDictionary(r => r.Id, r => r.RoomPresentation());

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
                            $"check-in \"{reservation.CheckInDate.ToShortDateString()}\", " +
                            $"check-out \"{reservation.CheckOutDate.ToShortDateString()}\",\n" +
                            $"\t\t\ttotal price \"{reservation.TotalPrice}\", " +
                            $"status \"{Enum.GetName(reservation.Status.GetType(), reservation.Status)}\"");
                    }
                }
            }
        }

        public Reservation? SelectReservation(User? user)
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

        public void EditReservation(User? admin, User? user, Reservation reservation)
        {
            List<Reservation> reservations = GetReservations();

            int statusIndex;
            RoomStatus? status = reservation.Status;
            //Hotel? hotel = null;
            //Room? room = null;
            //User? newUser = null;
            //DateTime? checkInDate = null;
            //DateTime? checkOutDate = null;

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

        public bool DeleteReservation(Reservation reservation)
        {
            List<Reservation> reservations = GetReservations();
            int index = reservations.FindIndex(r => r.Id == reservation.Id && r.HotelId == reservation.HotelId);

            if (index == -1)
            {
                Console.WriteLine("\t Reservation not found. Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            Console.Clear();
            
            Console.WriteLine($"\t{reservation.GetShortInfo()}");
            Console.Write($"\tDelete reservation? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            reservations.RemoveAt(index);
            dataHelper.WriteUpdateReservations(reservations);
            return true;
        }

        public void PrintReservationsManagmentHeader(User? admin, User? user, Reservation? reservation)
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

        private int GetMaxId(List<Reservation> list, int hotelId)
        {
            if (list.Count == 0)
                return 1;

            int maxId = list.Where(r => r.HotelId == hotelId).ToList().Max(x => x.Id);
            return ++maxId;
        }
    }
}
