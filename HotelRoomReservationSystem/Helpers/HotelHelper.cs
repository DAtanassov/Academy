using System.Text.Json;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class HotelHelper
    {
        private static DataHelper dataHelper = new DataHelper();
        private static RoomHelper roomHelper = new RoomHelper();

        public List<Hotel> GetHotels(int[]? hotelId = null)
        {

            string fileContent = dataHelper.GetFileContent("Hotels.json");

            List<Hotel>? hotels = new List<Hotel>();

            if (!String.IsNullOrEmpty(fileContent))
                hotels = JsonSerializer.Deserialize<List<Hotel>>(fileContent);

            if (hotels == null)
                return new List<Hotel>();

            if (hotelId != null)
                hotels = hotels.Where(h => hotelId.Contains(h.Id)).ToList();

            return hotels;
        }

        public Hotel? GetHotelById(int hotelId)
        {
            List<Hotel> hotels = GetHotels(new int[] { hotelId });
            if (hotels.Count == 0)
                return null;
            return hotels[0];
        }

        public void ShowAll()
        {
            Console.Clear();
            Console.WriteLine("\t\tHotels\n");

            List<Hotel> hotels = GetHotels();

            int counter = 0;
            foreach (Hotel h in hotels)
                Console.WriteLine($"\t{++counter}. {h.Name}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public Hotel? SelectHotel(Hotel? hotel)
        {

            List<Hotel> hotels = GetHotels();

            if (hotels.Count == 0)
                return null;

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
                        Console.WriteLine("\nInvalid option. Press any key to try again.");
                        Console.ReadKey();
                    }
                }
            }

            return hotel;
        }

        public Hotel? AddHotel()
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
            hotel.Id = GetMaxHotelId(hotels);

            hotels.Add(hotel);
            dataHelper.WriteUpdateHotels(hotels);

            return hotel;

        }

        public Hotel ChangeName(Hotel hotel)
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

            int hotelIndex = hotels.FindIndex(x => x.Id == hotel.Id);
            hotels[hotelIndex] = hotel;
            dataHelper.WriteUpdateHotels(hotels);

            return hotel;
        }

        public void DeleteHotel(Hotel hotel)
        {
            List<Hotel> hotels = GetHotels();

            int hotelIndex = hotels.FindIndex(x => x.Id == hotel.Id);

            Console.Clear();
            Console.Write($"\tDelete hotel \"{hotel.Name}\" and all data for the hotel? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return;

            dataHelper.DeleteHotelDataBase(hotel);
            hotels.RemoveAt(hotelIndex);
            dataHelper.WriteUpdateHotels(hotels);
        }

        public void EditHotel(Hotel? hotel)
        {
            
            if (hotel == null)
                return;
            
            bool running = true;

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
                        DeleteHotel(hotel);
                        running = false;
                        hotel = null;
                        break;
                    case "222": // Change name
                        ChangeName(hotel);
                        break;
                    case "300": // Room managment
                        RoomManagment(hotel);
                        break;
                    case "320": // Show hotel rooms
                        roomHelper.ShowRooms(hotel);
                        break;
                    case "321": // Show hotel room types
                        roomHelper.ShowRoomTypes(hotel);
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

        public void RoomManagment(Hotel hotel)
        {
            bool running = true;

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
                    case "320": // Show rooms
                        roomHelper.ShowRooms(hotel);
                        break;
                    case "321": // Show room types
                        roomHelper.ShowRoomTypes(hotel);
                        break;
                    case "322": // Add room
                        roomHelper.AddRoom(hotel);
                        break;
                    case "323": // Edit room
                        break;
                    case "324": // Delete room
                        break;
                    case "325": // Add room type
                        roomHelper.AddRoomType(hotel);
                        break;
                    case "326": // Edit room type
                        break;
                    case "327": // Delete room type
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

        private Dictionary<int, string[]> GetHotelEditMenu(bool isAdmin)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            if (isAdmin)
            {
                menu.Add(menu.Count + 1, ["Delete hotel", "220"]);
                menu.Add(menu.Count + 1, ["Change name", "221"]);
                menu.Add(menu.Count + 1, ["Room managment", "300"]);
            }
            menu.Add(menu.Count + 1, ["Show rooms", "320"]);
            menu.Add(menu.Count + 1, ["Show room types", "321"]);
            menu.Add(menu.Count + 1, ["< Back", "1000"]);
            return menu;
        }

        private Dictionary<int, string[]> GetHotelRoomManagmentMenu(bool isAdmin)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(menu.Count + 1, ["Show rooms", "320"]);
            menu.Add(menu.Count + 1, ["Show room types", "321"]);
            if (isAdmin)
            {
                menu.Add(menu.Count + 1, ["Add room", "322"]);
                menu.Add(menu.Count + 1, ["Edit room", "323"]);
                menu.Add(menu.Count + 1, ["Delete room", "324"]);

                menu.Add(menu.Count + 1, ["Add room type", "325"]);
                menu.Add(menu.Count + 1, ["Edit room type", "326"]);
                menu.Add(menu.Count + 1, ["Delete room type", "327"]);

            }
            menu.Add(menu.Count + 1, ["< Back", "1000"]);

            return menu;
        }

        private int GetMaxHotelId(List<Hotel> hotels)
        {
            if (hotels.Count == 0)
                return 1;

            int maxId = hotels.Max(x => x.Id);
            return ++maxId;
        }
    }
}
