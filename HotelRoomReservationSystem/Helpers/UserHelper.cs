using System.Net.Mail;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
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
                users = users.Where(u => (u.IsAdmin == true && !u.Deactivated)).ToList();
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

        private string GetEmailAddress()
        {
            string email = (Console.ReadLine() ?? string.Empty).Trim();
            while (!ValidateEmailAddress(email))
            {
                PrintAddUserHeader(null);
                Console.WriteLine("\tE-mail address is not valid!");
                Console.Write("\tEnter e-mail address: ");
                email = (Console.ReadLine() ?? string.Empty).Trim();
            }
            return email;
        }

        public User AddUser(bool userIsAdmin)
        {
            List<User> users = GetUsers();

            PrintAddUserHeader("\tName: ");
            string name = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrEmpty(name))
            {
                PrintAddUserHeader(null);
                Console.WriteLine("\tName cannot be empty!");
                Console.Write("\tName: ");
                name = Console.ReadLine() ?? string.Empty;
            }


            PrintAddUserHeader("\tE-mail address: ");
            string email = GetEmailAddress();
            while (users.Where(u => (u.Email == email.ToLower())).ToList().Count > 0)
            {
                PrintAddUserHeader(null);
                Console.WriteLine("\tThis e-mail address is registered!");
                Console.Write("\tEnter e-mail address: ");
                email = GetEmailAddress();
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
            Console.WriteLine($"{user.GetInfo(user)}");
            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();

            return user;

        }

        public bool EditUser(User admin, User user)
        {
            if (!(admin == user || admin.IsAdmin))
                return false;

            List<User> users = GetUsers();

            string name = user.Name;
            string email = user.Email;
            string username = user.Username;
            string password = user.Password;
            string phone = user.Phone;
            string address = user.Address;
            bool deactivated = user.Deactivated;

            // Edit name
            PrintUserManagmentHeader(admin, user, null);
            Console.Write("\tEdit name? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(name))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent name: \"{0}\"", user.Name);
                    Console.Write("\tNew name: ");
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
            Console.Write("\tEdit e-mail? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                email = "";
                while (string.IsNullOrEmpty(email))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent e-mail address: \"{0}\"", user.Email);
                    Console.Write("\tNew e-mail address: ");
                    email = GetEmailAddress();
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

                // Edit username
                if (!string.IsNullOrEmpty(email))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.Write("\tUse e-mail as username? (\"Y/n\"): ");
                    if ((Console.ReadLine() ?? "n").ToLower() == "y")
                        username = email;
                }
            }
            else
                email = user.Email;

            // Edit username
            PrintUserManagmentHeader(admin, user, null);
            Console.Write("\tEdit username? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {

                while (string.IsNullOrEmpty(username))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent username: \"{0}\"", user.Username);
                    Console.Write("\tNew username: ");
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
            Console.Write("\tEdit password? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(password))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.Write("\tEnter new password: ");
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
                        Console.Write("\tEnter new password again: ");
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
            Console.Write("\tEdit phone number? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(phone))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent phone number: \"{0}\"", user.Phone);
                    Console.Write("\tNew phone number: ");
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
            Console.Write("\tEdit address? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                while (string.IsNullOrEmpty(address))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent address: \"{0}\"", user.Address);
                    Console.Write("\tNew address: ");
                    address = Console.ReadLine() ?? string.Empty;
                    if (string.IsNullOrEmpty(address))
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.Write("\tAddress is empty! Continue? (\"Y/n\"): ");
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
                Console.Write($"\t{((user.Deactivated) ? "Activate" : "Deactivate")} user? (\"Y/n\"): ");
                if ((Console.ReadLine() ?? "n").ToLower() == "y")
                    deactivated = !user.Deactivated;
            }
            else
                deactivated = user.Deactivated;

            email = email.ToLower().Trim();

            if (user.Name == name && user.Email == email && user.Deactivated == deactivated
                && user.Username == username && user.Password == password
                && user.Phone == phone && user.Address == address)
            {
                Console.WriteLine("\tThe user not changed.");
                Console.WriteLine("\tPress any key to continue...");
                Console.ReadKey();
                return false;
            }

            user.Name = name;
            user.Email = email;
            user.Username = username; 
            user.Password = password;
            user.Phone = phone;
            user.Address = address;
            user.Deactivated = deactivated;

            
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                users.Add(user);
            else
                users[index] = user;

            dataHelper.WriteUpdateUsers(users);

            return true;
        }

        public void DeleteUser(User user)
        {
            Console.Clear();
            Console.Write($"\tDelete user \"{user.Name}\" and all data for the user? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return;

            List<User> users = GetUsers();
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                return;

            users.RemoveAt(index);
            dataHelper.WriteUpdateUsers(users);

        }

        public void ShowAll(User? admin = null)
        {
            Console.Clear();
            Console.WriteLine("\t\tUsers\n");

            List<User> users = GetUsers();
            if (admin  != null)
                users = users.Where(u => u.Id != ((User)admin).Id).ToList();

            int counter = 0;
            foreach (User u in users)
                Console.WriteLine($"\t{++counter}. {u.GetShortInfo(true)}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public User? SelectUser(User admin, User? user)
        {
            List<User> users = GetUsers();
            if (user != null)
                users = users.Where(u => u.Id != admin.Id && u.Id != ((User)user).Id).ToList();
            else
                users = users.Where(u => u.Id != admin.Id).ToList();

            if (users.Count == 0)
                return null;

            Console.Clear();
            Console.WriteLine($"\tUser accounts:\n");

            while (true)
            {
                int counter = 0;
                foreach (User u in users)
                    Console.WriteLine($"\t{++counter}. {u.GetShortInfo()}");

                Console.WriteLine($"\n\t{++counter}. Cancel");
                Console.Write("\n\n\tChoose user: ");

                int choice;
                if (int.TryParse((Console.ReadLine() ?? "0"), out choice))
                {
                    if (choice > 0 && choice <= users.Count)
                        return users[choice - 1];
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

        public void PrintUserProfileHeader(User? user, Reservation? reservation)
        {
            Console.Clear();

            // Print first row (reservation, User)
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

        public void PrintUserManagmentHeader(User? admin, User? user, Reservation? reservation)
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
                Console.Write($"\nUser: \"{user.Name}\"");
            if (reservation != null)
                Console.Write($"\nReservation: \"{reservation.Id.ToString()}\"");

            Console.WriteLine("\n\n");
        }

        private bool ValidateEmailAddress(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // TODO - Encrypt, Decript password

        private int GetMaxUserId(List<User> users)
        {
            if (users.Count == 0)
                return 1;

            int maxId = users.Max(x => x.Id);
            return ++maxId;
        }
    }

}
