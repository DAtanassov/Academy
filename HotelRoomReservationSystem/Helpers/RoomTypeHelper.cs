using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class RoomTypeHelper
    {
        private static List<RoomType> GetRoomTypes(int[]? hotelId = null)
        {
            List<RoomType>? roomTypes = DataHelper.GetRoomTypeList();

            if (roomTypes.Count > 0 && hotelId != null)
                roomTypes = roomTypes.Where(x => (hotelId.Contains(x.HotelId))).ToList();

            return roomTypes;
        }

        public static RoomType? GetRoomTypeById(int id, int hotelId)
        {
            List<RoomType> roomTypes = GetRoomTypes([hotelId]);
            return GetRoomTypeById(id, roomTypes);
        }

        private static RoomType? GetRoomTypeById(int id, List<RoomType> roomTypes)
        {
            roomTypes = roomTypes.Where(x => (x.Id == id)).ToList();

            if (roomTypes.Count > 0)
                return roomTypes[0];

            return null;
        }

        public static void ShowAllRoomTypes(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tRoom types in hotel \"{hotel.Name}\"\n");

            List<RoomType> roomTypes = GetRoomTypes([hotel.Id]);

            int counter = 0;
            foreach (RoomType rt in roomTypes)
                Console.WriteLine($"\t{++counter}. {rt.Name}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public static RoomType? AddRoomType(Hotel hotel)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\tAdding room type in hotel \"{hotel.Name}\"\n");

            List<RoomType> roomTypes = GetRoomTypes(null);

            Console.Write("\tRoom type name: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name cannot be empty!\nRoom type not created.");
                return null;
            }

            List<string> amenities = new List<string>();
            Console.Clear();
            Console.Write($"\tAdd room type amenities? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                string amenitiesName;
                while (true)
                {
                    Console.Clear();
                    Console.Write("\t");
                    amenitiesName = Console.ReadLine() ?? string.Empty;
                    if (!string.IsNullOrEmpty(amenitiesName))
                        amenities.Add(amenitiesName);

                    Console.Clear();
                    Console.Write($"\tAdd more? (\"Y/n\"): ");
                    if ((Console.ReadLine() ?? "n").ToLower() != "y")
                        break;
                }
            }

            Console.Clear();
            Console.Write("\tEnter maximum occupancy: ");
            int maximumOccupancy;
            if (!int.TryParse(Console.ReadLine() ?? "0", out maximumOccupancy))
                maximumOccupancy = 0;

            RoomType roomType = new RoomType(name, hotel.Id, amenities, maximumOccupancy);
            DataHelper.InsertHotelRoomTypes([roomType]);

            return roomType;
        }

        public static RoomType? SelectRoomType(int hotelId)
        {
            Hotel? hotel = HotelHelper.GetHotelById(hotelId);
            if (hotel != null)
                return SelectRoomType(hotel);
            return null;
        }

        public static RoomType? SelectRoomType(Hotel hotel)
        {
            List<RoomType> roomTypes = GetRoomTypes([hotel.Id]);

            if (roomTypes.Count == 0)
                return null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\t\tRoom types in hotel {hotel.Name}\n");

                int counter = 0;
                foreach (RoomType rt in roomTypes)
                    Console.WriteLine($"\t{++counter}. {rt.Name}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a room type: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= roomTypes.Count)
                        return roomTypes[choice - 1];
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

        public static void EditRoomType(RoomType roomType)
        {
            List<RoomType> roomTypes = GetRoomTypes();

            string name = roomType.Name;
            List<string> amenities = roomType.Amenities;
            int maximumOccupancy = roomType.MaximumOccupancy;

            // Edit name
            PrintRoomTypeEditHeader(roomType);
            Console.Write("\tEdit name? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(name))
                {
                    PrintRoomTypeEditHeader(roomType);
                    Console.WriteLine("\tCurrent name: \"{0}\"", roomType.Name);
                    Console.Write("New name: ");
                    name = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(name))
                    {
                        PrintRoomTypeEditHeader(roomType);
                        Console.WriteLine("\tName cannot be empty");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return;
                    }
                }
            }

            // TODO amenities

            // Edit maximum occupancy
            PrintRoomTypeEditHeader(roomType);
            Console.Write("\tEdit maximum occupancy? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (true)
                {
                    PrintRoomTypeEditHeader(roomType);
                    Console.WriteLine("\tCurrent price: \"{0}\"", roomType.MaximumOccupancy);
                    Console.Write("\tNew price: ");
                    if (int.TryParse(Console.ReadLine() ?? "0", out maximumOccupancy))
                    {
                        PrintRoomTypeEditHeader(roomType);
                        if (maximumOccupancy == 0 || maximumOccupancy < 0)
                        {
                            Console.Write("\tMaximum occupancy is \"{0}\". Continue? (\"Y/n\"): ", maximumOccupancy);
                            if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                break;
                            else
                                continue;
                        }
                        else if (maximumOccupancy > 0)
                            break;
                    }
                }
            }

            if (roomType.Name == name && roomType.MaximumOccupancy == maximumOccupancy
                && roomType.Amenities.Count == amenities.Count)
            {
                bool isEquals = true;
                foreach (string amen in amenities)
                {
                    if (!roomType.Amenities.Contains(amen))
                    {
                        isEquals = false;
                        break;
                    }
                }

                if (isEquals)
                {
                    Console.WriteLine("\tThe room not changed.");
                    Console.WriteLine("\tPress any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }

            roomType.Name = name;
            roomType.Amenities = amenities;
            roomType.MaximumOccupancy = maximumOccupancy;

            int index = roomTypes.FindIndex(r => r.Id == roomType.Id && r.HotelId == roomType.HotelId);
            if (index == -1)
                roomTypes.Add(roomType);
            else
                roomTypes[index] = roomType;

            DataHelper.UpdateHotelRoomTypes(roomTypes);

        }

        public static bool DeleteRoomType(RoomType roomType)
        {
            List<RoomType> roomTypes = GetRoomTypes();

            int index = roomTypes.FindIndex(r => r.Id == roomType.Id);
            if (index == -1)
            {
                return false;
            }

            Console.Clear();
            Console.Write($"\tDelete room \"{roomType.Name}\" and all data for the room? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            DataHelper.DeleteRoomTypeData(roomType.Id, roomType.HotelId);
            roomTypes.RemoveAt(index);
            DataHelper.UpdateHotelRoomTypes(roomTypes);

            return false;
        }

        private static void PrintRoomTypeEditHeader(RoomType roomType)
        {
            Console.Clear();

            // Print first row (Room)
            Console.Write($"Edit room type: \"{roomType.Name}\"");

            Console.WriteLine("\n\n");
        }

    }
}
