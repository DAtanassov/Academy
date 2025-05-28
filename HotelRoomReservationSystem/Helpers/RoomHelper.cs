using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class RoomHelper
    {
        public static List<Room> GetRooms(int[]? hotelId = null)
        {
            List<Room>? rooms = DataHelper.GetRoomList();

            if (rooms.Count > 0 && hotelId != null)
                rooms = rooms.Where(x => hotelId.Contains(x.HotelId)).ToList();

            return rooms;
        }

        public static Room? GetRoomById(int id, int hotelId)
        {
            List<Room> rooms = GetRooms([hotelId]);
            return GetRoomById(id, rooms);
        }

        private static Room? GetRoomById(int id, List<Room> rooms)
        {
            rooms = rooms.Where(x => (x.Id == id)).ToList();
            if (rooms.Count > 0)
                return rooms[0];
            return null;
        }

        public static void ShowAllRooms(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tRoom in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms([hotel.Id]);

            int counter = 0;
            foreach (Room r in rooms)
                Console.WriteLine($"\t{++counter}. {r.ShortInfo()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public static Room? AddRoom(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tAdding room in hotel \"{hotel.Name}\"\n");

            List<Room> rooms = GetRooms();

            Console.Write("\tRoom number: ");
            int number;
            if (!int.TryParse(Console.ReadLine(), out number))
            {
                Console.WriteLine("Number cannot be empty!\nRoom not created.");
                return null;
            }

            RoomType? roomType = RoomTypeHelper.SelectRoomType(hotel);

            if (roomType == null)
            {
                Console.WriteLine("Room type cannot be empty!\nRoom not created.\n" +
                    "Press any key to continue...");
                Console.ReadKey();
                return null;
            }

            Console.Clear();
            Console.Write("\tEnter price per night: ");
            string inputText = Console.ReadLine() ?? "0.00";
            decimal pricePerNight;
            if (!decimal.TryParse(inputText, out pricePerNight))
                if ((inputText.Contains('.') && decimal.TryParse(inputText.Replace('.', ','), out pricePerNight))
                    || (inputText.Contains(',') && decimal.TryParse(inputText.Replace(',', '.'), out pricePerNight))) { }



            Console.Clear();
            Console.Write("\tEnter cancellation fee: ");
            inputText = Console.ReadLine() ?? "0.00";
            decimal cancellationFee;
            if (!decimal.TryParse(inputText, out cancellationFee))
                if ((inputText.Contains('.') && decimal.TryParse(inputText.Replace('.', ','), out cancellationFee))
                    || (inputText.Contains(',') && decimal.TryParse(inputText.Replace(',', '.'), out cancellationFee))) { }


            Room room = new Room(number, roomType.Id, hotel.Id, pricePerNight, cancellationFee);
            DataHelper.InsertHotelRooms([room]);

            return room;
        }

        public static Room? SelectRoom(Hotel hotel)
        {
            List<Room> rooms = GetRooms([hotel.Id]);

            if (rooms.Count == 0)
                return null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t\tRooms in hotel {hotel.Name}\n");

                int counter = 0;
                foreach (Room r in rooms)
                    Console.WriteLine($"\t{++counter}. {r.ShortInfo()}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a room: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= rooms.Count)
                        return rooms[choice - 1];
                    else if (choice == counter)
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                    }
                }
            }

            return null;
        }

        public static void EditRoom(Room room)
        {
            List<Room> rooms = GetRooms();
            List<Room> hotelRooms = rooms.Where(r => r.HotelId == room.HotelId && r.Id != room.Id).ToList();

            RoomType? roomType = RoomTypeHelper.GetRoomTypeById(room.RoomTypeId, room.HotelId);

            int number = room.Number;
            int roomTypeId = room.RoomTypeId;
            decimal pricePerNight = room.PricePerNight;
            decimal cancellationFee = room.CancellationFee;

            // Edit number
            PrintRoomEditHeader(room);
            Console.Write("\tEdit number? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomEditHeader(room);
                    Console.WriteLine("\tCurrent room number: \"{0}\"", room.Number);
                    Console.Write("\tNew number: ");
                    if (int.TryParse(Console.ReadLine() ?? "", out number))
                    {
                        PrintRoomEditHeader(room);
                        if (number == 0)
                            Console.WriteLine("\tNumber must be greather than \"0\"");
                        else if (hotelRooms.Where(r => r.Id == number).ToList().Count > 0)
                            Console.WriteLine("\tNumber {0} is used!", number);
                        else
                            break;

                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return;
                    }
                }
            }

            // Edit Room type
            PrintRoomEditHeader(room);
            Console.Write("\tEdit room type? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                RoomType? newRoomType = null;

                while (newRoomType == null)
                {
                    PrintRoomEditHeader(room);
                    newRoomType = RoomTypeHelper.SelectRoomType(room.HotelId);

                    if (newRoomType == null)
                    {
                        PrintRoomEditHeader(room);
                        Console.WriteLine("\tRoom type cannot be empty!");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            break;
                    }
                    else
                        roomType = newRoomType;
                }
            }

            // Edit pricePerNight
            PrintRoomEditHeader(room);
            Console.Write("\tEdit price per night? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomEditHeader(room);
                    Console.WriteLine("\tCurrent price: \"{0}\"", room.PricePerNight);
                    Console.Write("\tNew price: ");
                    if (decimal.TryParse(Console.ReadLine() ?? "0.00", out pricePerNight))
                    {
                        PrintRoomEditHeader(room);
                        if (pricePerNight == 0m || pricePerNight < 0m)
                        {
                            Console.Write("\tPrice per night is \"{0}\". Continue? (\"Y/n\"): ", pricePerNight);
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                break;
                            else
                                continue;
                        }
                        else if (pricePerNight > 0m)
                            break;
                    }
                }
            }

            // Edit cancellationFee
            PrintRoomEditHeader(room);
            Console.Write("\tEdit cancellation fee? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomEditHeader(room);
                    Console.WriteLine("\tCurrent price: \"{0}\"", room.CancellationFee);
                    Console.Write("\tNew price: ");
                    if (decimal.TryParse(Console.ReadLine() ?? "0.00", out cancellationFee))
                    {
                        PrintRoomEditHeader(room);
                        if (cancellationFee == 0m || cancellationFee < 0m)
                        {
                            Console.Write("\tCancellation fee is \"{0}\". Continue? (\"Y/n\"): ", cancellationFee);
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                break;
                            else
                                continue;
                        }
                        else if (cancellationFee > 0m)
                            break;
                    }
                }
            }

            if ((room.Number == number && room.RoomTypeId == roomTypeId
                && room.PricePerNight == pricePerNight && room.CancellationFee == cancellationFee))
            {
                Console.WriteLine("\tThe room not changed.");
                Console.WriteLine("\tPress any key to continue...");
                Console.ReadKey();
                return;
            }

            room.Number = number;
            room.RoomTypeId = roomTypeId;
            room.PricePerNight = pricePerNight;
            room.CancellationFee = cancellationFee;

            int index = rooms.FindIndex(r => r.Id == room.Id && r.HotelId == room.HotelId);
            if (index == -1)
                rooms.Add(room);
            else
                rooms[index] = room;

            DataHelper.UpdateHotelRooms(rooms);

        }

        public static bool DeleteRoom(Room room)
        {
            List<Room> rooms = GetRooms();

            int index = rooms.FindIndex(r => r.Id == room.Id);
            if (index == -1)
            {
                return false;
            }

            Console.Clear();
            Console.Write($"\tDelete room \"{room.Info()}\" and all data for the room? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            DataHelper.DeleteRoomData(room.Id, room.HotelId);
            rooms.RemoveAt(index);
            DataHelper.UpdateHotelRooms(rooms);

            return true;
        }

        private static void PrintRoomEditHeader(Room room)
        {
            Console.Clear();

            // Print first row (Room)
            Console.Write($"Edit room: \"{room.ShortInfo()}\"");

            Console.WriteLine("\n\n");
        }

    }
}
