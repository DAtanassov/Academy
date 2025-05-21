using System.ComponentModel.Design;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class UserHelper
    {
        private static DataHelper dataHelper = new DataHelper();

        private List<User> GetUsers()
        {
            string fileContent = dataHelper.GetFileContent("Users.json");

            List<User>? users = new List<User>();

            if (!String.IsNullOrEmpty(fileContent))
                users = JsonSerializer.Deserialize<List<User>>(fileContent);

            if (users == null)
                return new List<User>();

            return users;
        }

        public bool isAdminRegistered()
        {
            List<User> users = GetUsers();

            if (users.Count > 0)
            {
                users = users.Where(u => (u.IsAdmin == true)).ToList();
                return (users.Count > 0);
            }

            return false;
        }

        public User? GetUser(string username, string password)
        {
            List<User> users = GetUsers();
            users = users.Where(u => (u.Username == username && u.Password == password)).ToList();
            if (users.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("\n\tIncorrect username or password!");
                Console.WriteLine("\tPress any key to continue...");
                Console.ReadKey();
                return null;
            }

            User user = users[0];
            if (user.Deactivated)
            {
                Console.Clear();
                Console.WriteLine("\n\tUser is deactivated!");
                Console.WriteLine("\tPress any key to continue...");
                Console.ReadKey();
                return null;
            }

            return user;
        }

        public User? UserLogin()
        {
            Console.Clear();
            Console.Write("\n\tUsername: ");
            string user = Console.ReadLine() ?? "";
            Console.Clear();
            Console.Write("\n\tPassword: ");
            string pass = Console.ReadLine() ?? "";
            
            return GetUser(user, pass);

        }

        private static void PrintAddUserHeader(string? additionalText)
        {
            Console.Clear();
            Console.WriteLine("\t\tAdd user\n\n");
            if (!string.IsNullOrEmpty(additionalText))
                Console.Write(additionalText);
        }

        public User AddUser(bool userIsAdmin)
        {
            List<User> users = GetUsers();

            PrintAddUserHeader("\tName: ");
            string name = Console.ReadLine() ?? string.Empty;

            PrintAddUserHeader("\tE-mail address: ");
            string email = (Console.ReadLine() ?? string.Empty).Trim();
            while (users.Where(u => (u.Email == email.ToLower())).ToList().Count > 0)
            {
                PrintAddUserHeader(null);
                Console.WriteLine("\tThis e-mail address is registered!");
                Console.Write("\tEnter e-mail address: ");
                email = (Console.ReadLine() ?? string.Empty).Trim();
            }

            string username = "";
            if (!string.IsNullOrEmpty(email))
            {
                PrintAddUserHeader("\tUse e-mail as username? (\"Y/n\"): ");
                if ((Console.ReadLine() ?? "n").ToLower() == "y")
                    username = email.ToLower();
            }

            if (string.IsNullOrEmpty(username))
            {
                PrintAddUserHeader("\tUsername: ");
                username = (Console.ReadLine() ?? string.Empty).Trim();
            }
            while (users.Where(u => (u.Username == username)).ToList().Count > 0)
            {
                PrintAddUserHeader(null);
                Console.WriteLine("\tUsername is used!");
                Console.Write("\tEnter new username: ");
                username = (Console.ReadLine() ?? string.Empty).Trim();
            }

            PrintAddUserHeader("\tPassword: ");
            string password = Console.ReadLine() ?? "1234";

            User user = new User(name, email, username, password, userIsAdmin);
            user.Id = GetMaxUserId(users);

            users.Add(user);
            dataHelper.WriteUpdateUsers(users);

            PrintAddUserHeader("\tCreated user:\n");
            Console.WriteLine($"{user.UserInfo(user)}");
            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();

            return user;

        }

        private bool EditUser(User admin, User user)
        {
            if (!(admin == user || admin.IsAdmin))
                return false;

            List<User> users = GetUsers();

            string name = "";
            string email = "";
            string username = "";
            string password = "";
            string phone = "";
            string address = "";
            bool deactivated = false;

            // Edit name
            PrintUserManagmentHeader(admin, user, null);
            Console.WriteLine("\tEdit name? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(name))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent name: \"{0}\"", user.Name);
                    Console.Write("New name: ");
                    name = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(name))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tName cannot be empty");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return false;
                    }
                }
            }
            else
                name = user.Name;


            // Edit e-mail address
            PrintUserManagmentHeader(admin, user, null);
            Console.WriteLine("\tEdit e-mail? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(email))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent e-mail address: \"{0}\"", user.Email);
                    Console.Write("New e-mail address: ");
                    email = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(email))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tE-mail address cannot be empty");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return false;
                    }
                    else
                    {
                        email = email.ToLower().Trim();
                        if (users.Where(u => (u.Id != user.Id && u.Email == email)).ToList().Count() > 0)
                        {
                            PrintUserManagmentHeader(admin, user, null);
                            Console.WriteLine("\tE-mail \"{0}\" address is alredy used", email);
                            email = "";
                            Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                            ConsoleKeyInfo userInput = Console.ReadKey();
                            if (userInput.Key == ConsoleKey.Escape)
                                return false;
                        }
                    }
                }

                // Edit username
                if (!string.IsNullOrEmpty(email))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tUse e-mail as username? (\"Y/n\"): ");
                    if ((Console.ReadLine() ?? "n").ToLower() == "y")
                        username = email;
                }
            }
            else
                email = user.Email;

            // Edit username
            PrintUserManagmentHeader(admin, user, null);
            Console.WriteLine("\tEdit username? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {

                while (string.IsNullOrEmpty(username))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent username: \"{0}\"", user.Username);
                    Console.Write("New username: ");
                    username = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(username))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tUsername cannot be empty");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return false;
                    }
                    else
                    {
                        username = username.ToLower().Trim();
                        if (users.Where(u => (u.Id != user.Id && u.Username == username)).ToList().Count() > 0)
                        {
                            PrintUserManagmentHeader(admin, user, null);
                            Console.WriteLine("\tUssername \"{0}\" is alredy used", username);
                            username = "";
                            Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                            ConsoleKeyInfo userInput = Console.ReadKey();
                            if (userInput.Key == ConsoleKey.Escape)
                                return false;
                        }
                    }
                }
            }
            else
                username = user.Username;


            // Edit password
            PrintUserManagmentHeader(admin, user, null);
            Console.WriteLine("\tEdit password? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(password))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.Write("Enter new password: ");
                    password = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(password))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tPassword cannot be empty");
                        Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                        ConsoleKeyInfo userInput = Console.ReadKey();
                        if (userInput.Key == ConsoleKey.Escape)
                            return false;
                    }
                    else
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.Write("Enter new password again: ");
                        if (password != (Console.ReadLine() ?? string.Empty))
                        {
                            PrintUserManagmentHeader(admin, user, null);
                            Console.WriteLine("\tPasswords not matched");
                            password = "";
                            Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                            ConsoleKeyInfo userInput = Console.ReadKey();
                            if (userInput.Key == ConsoleKey.Escape)
                                return false;
                        }
                    }
                }
            }
            else
                password = user.Password;

            // Edit phone
            PrintUserManagmentHeader(admin, user, null);
            Console.WriteLine("\tEdit phone number? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(phone))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent phone number: \"{0}\"", user.Phone);
                    Console.Write("New phone number: ");
                    phone = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(phone))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tPhone number is empty! Continue? (\"Y/n\"): ");
                        if ((Console.ReadLine() ?? "n").ToLower() == "y")
                            break;
                    }
                }
            }
            else
                phone = user.Phone;

            // Edit address
            PrintUserManagmentHeader(admin, user, null);
            Console.WriteLine("\tEdit address? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(address))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent address: \"{0}\"", user.Address);
                    Console.Write("New address: ");
                    address = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(address))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tAddress is empty! Continue? (\"Y/n\"): ");
                        if ((Console.ReadLine() ?? "n").ToLower() == "y")
                            break;
                    }
                }
            }
            else
                address = user.Address;

            if (admin.IsAdmin && admin != user)
            {
                PrintUserManagmentHeader(admin, user, null);
                Console.WriteLine($"\t{((user.Deactivated) ? "Activate" : "Deactivate")} user? (\"Y/n\"): ");
                if ((Console.ReadLine() ?? "n").ToLower() == "y")
                    deactivated = !user.Deactivated;
            }
            else
                deactivated = user.Deactivated;

            user.Name = name;
            user.Email = email.ToLower().Trim(); 
            user.Username = username; 
            user.Password = password;
            
            user.Phone = phone;
            user.Address = address;
            user.Deactivated = deactivated;

            
            int index = users.FindIndex(u => u.Id == user.Id);
            users[index] = user;
            dataHelper.WriteUpdateUsers(users);

            return true;
        }

        private void DeleteUser(User user)
        {
            List<User> users = GetUsers();
            int index = users.FindIndex(u => u.Id == user.Id);
            users.RemoveAt(index);
            dataHelper.WriteUpdateUsers(users);

        }

        private Dictionary<int, string[]> GetUserProfileMenu(User? user, Reservation? reservation)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(0, ["Login/Logout", "0"]);

            if (user != null)
            {
                menu.Add(menu.Count, ["View profile", "1"]);
                menu.Add(menu.Count, ["Edit profile", "2"]);
                menu.Add(menu.Count, ["Delete profile", "3"]);

                if (user.IsAdmin)
                    menu.Add(menu.Count, ["Users managment", "4"]);

                menu.Add(menu.Count, ["Reservations menu", "5"]);
            }
            menu.Add(menu.Count, ["< Back", "1000"]);

            return menu;
        }
        
        private static void PrintUserProfileHeader(User? user, Reservation? reservation)
        {
            Console.Clear();

            // Print first row (Hotel, User)
            if (reservation == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Edit reservation ID: \"{reservation.Id.ToString()}\"");

            if (user == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{user.Name}\" (0. Logout)");

            Console.WriteLine("\n\n");
        }

        public void UserProfileMenu(User? user, Reservation? reservation)
        {
            bool running = true;

            ReservationHelper reservationHelper = new ReservationHelper();

            while (running)
            {
                PrintUserProfileHeader(user, reservation);

                Dictionary<int, string[]> menu = GetUserProfileMenu(user, reservation);

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
                        if (user == null)
                            user = UserLogin();
                        else
                            user = null;
                        break;
                    case "1": // View profile
                        if (user != null)
                        {
                            PrintUserProfileHeader(user, reservation);
                            Console.WriteLine(user.UserInfo(user));
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "2": // Edit profile
                        if (user != null && EditUser(user, user))
                        {
                            PrintUserProfileHeader(user, reservation);
                            Console.WriteLine("\tProfile edited successfully!");
                            user.UserInfo(user);
                        }
                        else
                        {
                            PrintUserProfileHeader(user, reservation);
                            Console.WriteLine("\tProfile not changed.");
                        }
                        Console.WriteLine("\n\tPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "3": // Delete profile
                        if (user != null)
                        {
                            PrintUserProfileHeader(user, reservation);
                            string name = user.Name;
                            DeleteUser(user);
                            running = false;
                            user = null;
                            reservation = null;
                            Console.WriteLine($"\tUser \"{name}\" delete successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "4": // Users managment (admin)
                        UserManagmentMenu(user, reservation);
                        break;
                    case "5": // Reservations menu
                        reservationHelper.ReservationManagmentMenu(user, user, reservation);
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

        private Dictionary<int, string[]> GetUserManagmentMenu(User? user, Reservation? reservation)
        {
            Dictionary<int, string[]> menu = new Dictionary<int, string[]>();

            menu.Add(0, ["Login/Logout", "0"]);

            if (user != null)
            {
                menu.Add(menu.Count, ["View users", "1"]);
                menu.Add(menu.Count, ["Add user", "2"]);
                if (user == null)
                    menu.Add(menu.Count, ["Select user", "3"]);
                else
                {
                    menu.Add(menu.Count, ["Deselect user", "4"]);
                    menu.Add(menu.Count, ["Edit user", "5"]);
                    menu.Add(menu.Count, ["Delete user", "6"]);
                    menu.Add(menu.Count, ["Reservations menu", "7"]);
                }
            }
            menu.Add(menu.Count, ["< Back", "1000"]);

            return menu;
        }

        public void UserManagmentMenu(User? admin, Reservation? reservation)
        {
            bool running = true;

            User? user = null;

            ReservationHelper reservationHelper = new ReservationHelper();

            while (running)
            {
                PrintUserManagmentHeader(admin, user, reservation);

                Dictionary<int, string[]> menu = GetUserManagmentMenu(user, reservation);

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
                    case "1": // Add user
                        AddUser(false);
                        break;
                    case "2": // View users
                        break;
                    case "3": // Select user
                        break;
                    case "4": // Deselect user
                        break;
                    case "5": // Edit user
                        if (user != null && EditUser(admin, user))
                        {
                            PrintUserManagmentHeader(admin, user, reservation);
                            Console.WriteLine("\tProfile edited successfully!");
                            user.UserInfo(user);
                        }
                        else
                        {
                            PrintUserManagmentHeader(admin, user, reservation);
                            Console.WriteLine("\tProfile not changed.");
                        }
                        Console.WriteLine("\n\tPress any key to continue...");
                        Console.ReadKey();
                        break;
                    case "6": // Delete user
                        if (user != null && admin != user)
                        {
                            PrintUserManagmentHeader(admin, user, reservation);
                            string name = user.Name;
                            DeleteUser(user);
                            user = null;
                            reservation = null;
                            Console.WriteLine($"\tUser \"{name}\" delete successfully.");
                            Console.WriteLine("\n\tPress any key to continue...");
                            Console.ReadKey();
                        }
                        break;
                    case "7": // Reservations menu
                        reservationHelper.ReservationManagmentMenu(admin, user, reservation);
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

        private static void PrintUserManagmentHeader(User? admin, User? user, Reservation? reservation)
        {
            Console.Clear();

            // Print first row (Hotel, User)
            if (reservation == null && user == null)
                Console.Write($"\t\t");
            else
                Console.Write($"Edit:\t\t");

            if (admin == null)
                Console.Write($"\t\t0. Login");
            else
                Console.Write($"\t\tUser: \"{admin.Name}\" (0. Logout)");

            // Print second row
            if (user != null)
                Console.Write($"User: \"{user.Name}\"");
            if (reservation != null)
                Console.Write($"Reservation: \"{reservation.Id.ToString()}\"");

            Console.WriteLine("\n\n");
        }

        private int GetMaxUserId(List<User> users)
        {
            if (users.Count == 0)
                return 1;

            int maxId = users.Max(x => x.Id);
            return ++maxId;
        }
    }

}
