using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class HotelHelper
    {
        public static List<Hotel> GetHotels(int[]? hotelId = null)
        {
            List<Hotel>? hotels = DataHelper.GetHotelList();

            if (hotelId != null)
                hotels = hotels.Where(h => hotelId.Contains(h.Id)).ToList();

            return hotels;
        }

        public static Hotel? GetHotelById(int hotelId)
        {
            List<Hotel> hotels = GetHotels([hotelId]);
            if (hotels.Count == 0)
                return null;
            return hotels[0];
        }

        public static void ShowAll()
        {
            Console.Clear();
            Console.WriteLine("\t\tHotels\n");

            List<Hotel> hotels = GetHotels();

            int counter = 0;
            foreach (Hotel h in hotels)
                Console.WriteLine($"\t{++counter}. {h.Info()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public static Hotel? SelectHotel(Hotel? hotel = null)
        {
            List<Hotel> hotels = GetHotels();

            if (hotels.Count == 0)
                return hotel;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\t\tHotels\n");

                int counter = 0;
                foreach (Hotel h in hotels)
                    Console.WriteLine($"\t{++counter}. {h.Name}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose a hotel: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= hotels.Count)
                        return hotels[choice - 1];
                    else if (choice == counter)
                        break;
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\nInvalid option. Press \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            break;

                    }
                }
            }

            return hotel;
        }

        public static Hotel? AddHotel()
        {
            List<Hotel> hotels = GetHotels();

            Console.Clear();
            Console.WriteLine("\t\tCreating Hotel\n");
            Console.Write("\tHotel name: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
                return null;

            Console.Clear();
            Console.WriteLine("\t\tCreating Hotel\n");
            Console.Write("\tHotel address: ");
            string address = Console.ReadLine() ?? string.Empty;

            Hotel hotel = new Hotel(name, address);
            DataHelper.InsertHotels([hotel]);

            return hotel;

        }

        public static Hotel ChangeName(Hotel hotel)
        {
            List<Hotel> hotels = GetHotels();

            Console.Clear();
            Console.WriteLine($"\t\tChanging name of hotel {hotel.Name}\n");
            Console.Write("\tHotel name: ");
            string name = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("\tName cannot be empty! Name not changed.");
                return hotel;
            }
            if (hotel.Name == name)
            {
                return hotel;
            }

            hotel.Name = name;

            int index = hotels.FindIndex(x => x.Id == hotel.Id);
            if (index == -1)
                hotels.Add(hotel);
            else
                hotels[index] = hotel;

            DataHelper.UpdateHotels(hotels);

            return hotel;
        }

        public static bool DeleteHotel(Hotel hotel)
        {
            List<Hotel> hotels = GetHotels();

            int index = hotels.FindIndex(h => h.Id == hotel.Id);
            if (index == -1)
            {
                Console.WriteLine("\t Hotel not found. Press any key to continue...");
                Console.ReadKey();
                return false;

            }
            
            Console.Clear();
            Console.Write($"\tDelete hotel \"{hotel.Name}\" and all data for the hotel? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return false;

            DataHelper.DeleteHotelData(hotel.Id);
            hotels.RemoveAt(index);
            DataHelper.UpdateHotels(hotels);

            return true;
        }

        private static List<Hotel> GetManageringHotels(int userId)
        {
            List<Hotel> hotels = GetHotels();

            return hotels.Where(h => h.ManagerId == userId).ToList();

        }

        public static bool UserIsHotelManager(int userId, int hotelId = 0)
        {
            List<Hotel> hotels = GetManageringHotels(userId);
            if (hotelId == 0)
                return hotels.Count > 0;

            return hotels.Where(h => h.Id == hotelId).Count() > 0;
        }
        
    }
}
