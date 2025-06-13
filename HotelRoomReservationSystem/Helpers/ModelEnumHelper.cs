using System.ComponentModel;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class ModelEnumHelper
    {
        public static RoomStatus SelectStatus(RoomStatus status)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.available) ]);
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.booked) ]);
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.ocupated) ]);
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.canceled) ]);
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.expired) ]);
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.completed) ]);
            menu.Add(menu.Count + 1, [ GetDescription(RoomStatus.completed) ]);
            menu.Add(menu.Count + 1, ["Cancel"]);



            bool running = true;
            MenuHelper menuHelper = new MenuHelper();

            Console.CursorVisible = false;
            menuHelper.PrintAppName();
            Console.WriteLine("\t\tSelect a status:\n");
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);
                menuHelper.PrintMenuElements(menu, menuParams, false);
                menuParams.key = Console.ReadKey(true);

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
                            status = (RoomStatus)menuParams.choice - 1;
                        running = false;
                        break;
                }

            }
            return status;
        }

        public static string GetDescription<T>(T enumValue) where T : Enum
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != null)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }
            return enumValue.ToString();
        }
    }
}
