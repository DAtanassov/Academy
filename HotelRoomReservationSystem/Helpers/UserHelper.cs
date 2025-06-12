using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    public class UserHelper
    {
        public static List<User> GetUsers()
            => DataHelper.GetUserList();

        public static List<User> GetUsers(int[] userId)
        {
            List<User> users = GetUsers();

            if (userId.Length > 0)
                users = users.Where(u => userId.Contains(u.Id)).ToList();

            return users;
        }

        public static bool isAdminRegistered()
        {
            List<User> users = DataHelper.GetUserList();

            if (users.Count > 0)
            {
                users = users.Where(u => (u.IsAdmin == true && !u.Deactivated)).ToList();
                return (users.Count > 0);
            }

            return false;
        }

        public static User? GetUser(string username, string password)
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

        public User? GetUserById(int userId)
        {
            List<User> users = GetUsers([userId]);
            return GetUserById(users, userId);
        }

        public User? GetUserById(List<User> users, int id)
        {
            if (users.Count == 0)
                return null;
            return users.FirstOrDefault(h => h.Id == id);
        }

        public static User? UserLogin()
        {
            Console.Clear();
            Console.Write("\n\tUsername: ");
            string user = Console.ReadLine() ?? "";
            Console.Clear();
            Console.Write("\n\tPassword: ");
            string pass = Console.ReadLine() ?? "";
            
            return GetUser(user, pass);

        }

        /*
        private static string GetEmailAddress()
        {
            string email = (Console.ReadLine() ?? string.Empty).Trim();
            while (!Validator.EmailValidate(email))
            {
                new MenuHelper().PrintAddUserHeader();
                Console.WriteLine("\tEmail address is not valid!");
                Console.Write("\tEnter email address: ");
                email = (Console.ReadLine() ?? string.Empty).Trim();
            }
            return email;
        }
        */

        public bool AddUser(bool userIsAdmin)
            => AddEditUser(Program.user, new User(), userIsAdmin);
        /*
        {
            MenuHelper menuHelper = new MenuHelper();

            List<User> users = DataHelper.GetUserList();

            menuHelper.PrintAddUserHeader("\tName: ");
            string name = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrEmpty(name))
            {
                menuHelper.PrintAddUserHeader();
                Console.WriteLine("\tName cannot be empty!");
                Console.Write("\tName: ");
                name = Console.ReadLine() ?? string.Empty;
            }


            menuHelper.PrintAddUserHeader("\tEmail address: ");
            string email = GetEmailAddress();
            while (users.Where(u => (u.Email == email.ToLower())).ToList().Count > 0)
            {
                menuHelper.PrintAddUserHeader();
                Console.WriteLine("\tThis email address is registered!");
                Console.Write("\tEnter email address: ");
                email = GetEmailAddress();
            }

            string username = "";
            if (!string.IsNullOrEmpty(email))
            {
                menuHelper.PrintAddUserHeader("\tUse email as username? (\"Y/n\"): ");
                if ((Console.ReadLine() ?? "n").ToLower() == "y")
                    username = email.ToLower();
            }

            if (string.IsNullOrEmpty(username))
            {
                menuHelper.PrintAddUserHeader("\tUsername: ");
                username = (Console.ReadLine() ?? string.Empty).Trim();
            }
            while (users.Where(u => (u.Username == username)).ToList().Count > 0)
            {
                menuHelper.PrintAddUserHeader();
                Console.WriteLine("\tUsername is used!");
                Console.Write("\tEnter new username: ");
                username = (Console.ReadLine() ?? string.Empty).Trim();
            }

            menuHelper.PrintAddUserHeader("\tPassword: ");
            string password = Console.ReadLine() ?? "1234";

            User user = new User(name, email, username, password, userIsAdmin);
            DataHelper.InsertUsers([user]);

            menuHelper.PrintAddUserHeader("\tCreated user:\n");
            Console.WriteLine($"{user.Info()}");
            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();

            return user;

        }
        */

        public bool EditUser(User admin, User user)
            => AddEditUser(admin, user, false);
        /*
        {
            if (!(admin.CompareTo(user) == 0 || admin.IsAdmin))
                return false;

            List<User> users = DataHelper.GetUserList();

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


            // Edit email address
            PrintUserManagmentHeader(admin, user, null);
            Console.Write("\tEdit email? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() == "y")
            {
                email = "";
                while (string.IsNullOrEmpty(email))
                {
                    PrintUserManagmentHeader(admin, user, null);
                    Console.WriteLine("\tCurrent email address: \"{0}\"", user.Email);
                    Console.Write("\tNew email address: ");
                    email = GetEmailAddress();
                    email = email.ToLower().Trim();
                    if (users.Where(u => (u.Id != user.Id && u.Email == email)).ToList().Count() > 0)
                    {
                        PrintUserManagmentHeader(admin, user, null);
                        Console.WriteLine("\tEmail \"{0}\" address is alredy used", email);
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
                    Console.Write("\tUse email as username? (\"Y/n\"): ");
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

            if ((user.Name == name && user.Email == email && user.Deactivated == deactivated
                && user.Username == username && user.Password == password
                && user.Phone == phone && user.Address == address))
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

            DataHelper.UpdateUsers(users);

            return true;
        }
        */

        private bool AddEditUser(User? admin, User user, bool userIsAdmin)
        {
            bool addNew = (user.Id == 0);

            if (!addNew && admin != null && !(admin.CompareTo(user) == 0 || admin.IsAdmin))
                return false;

            bool isAdmin = (admin != null && admin.IsAdmin);

            if (addNew && userIsAdmin)
                user.IsAdmin = true;

            List<User> users = GetUsers();

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            UserHelper userHelper = new UserHelper();

            menuHelper.PrintAppName();
            string title = $"\t\t{(addNew ? "Creat" : "Edit")} User\n";
            Console.WriteLine(title);

            
            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();
            bool cancel = false;
            bool running = true;
            while (running)
            {
                Console.SetCursorPosition(menuParams.left, menuParams.top);

                Console.WriteLine($"\t{(menuParams.choice == 1 ? menuParams.prefix : "  ")}1. Name: {user.Name}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 2 ? menuParams.prefix : "  ")}2. Username: {user.Username}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 3 ? menuParams.prefix : "  ")}3. Password: **** \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 4 ? menuParams.prefix : "  ")}4. Email: {user.Email}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 5 ? menuParams.prefix : "  ")}5. Phone: {user.Phone}\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 6 ? menuParams.prefix : "  ")}6. Address: {user.Address} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 7 ? (isAdmin ? menuParams.prefix : "• ") : "  ")}7. Administrator: {(user.IsAdmin ? "Yes" : "No")} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 8 ? (isAdmin ? menuParams.prefix : "• ") : "  ")}8. Deactivated: {(user.Deactivated ? "Yes" : "No")} \u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 9 ? menuParams.prefix : "  ")}9. Save\u001b[0m");
                Console.WriteLine($"\t{(menuParams.choice == 10 ? menuParams.prefix : "  ")}10. Cancel\u001b[0m");

                menuParams.key = Console.ReadKey(false);

                switch (menuParams.key.Key)
                {
                    case ConsoleKey.UpArrow:
                        
                        if (isAdmin)
                            menuParams.choice = menuParams.choice == 1 ? 10 : menuParams.choice - 1;
                        else if (menuParams.choice == 9)
                            menuParams.choice = 6;
                        else
                            menuParams.choice = menuParams.choice == 1 ? 10 : menuParams.choice - 1;
                        continue;

                    case ConsoleKey.DownArrow:
                        if (isAdmin)
                            menuParams.choice = menuParams.choice == 10 ? 1 : menuParams.choice + 1;
                        else if (menuParams.choice == 6)
                            menuParams.choice = 9;
                        else
                            menuParams.choice = menuParams.choice == 1 ? 10 : menuParams.choice - 1;

                        continue;

                    case ConsoleKey.Enter:

                        Console.CursorVisible = true;

                        switch(menuParams.choice)
                        {
                            case 1:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tName: ");
                                user.Name = Console.ReadLine() ?? string.Empty;
                                while (!Validator.NameValidate(user.Name, user.Id, users))
                                {
                                    if (string.IsNullOrEmpty(user.Name))
                                        Console.WriteLine("\tName cannot be empty!");
                                    else
                                        Console.WriteLine("\tName is used!");
                                    Console.Write("\tName: ");
                                    user.Name = Console.ReadLine() ?? string.Empty;
                                }
                                break;
                            case 2:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tUsername: ");
                                user.Username = Console.ReadLine() ?? string.Empty;
                                while (!Validator.UsernameValidate(user.Username, user.Id, users))
                                {
                                    if (string.IsNullOrEmpty(user.Username))
                                        Console.WriteLine("\tUsername cannot be empty!");
                                    else
                                        Console.WriteLine("\tUsername is used!");
                                    Console.Write("\tUsername: ");
                                    user.Username = Console.ReadLine() ?? string.Empty;
                                }
                                break;
                            case 3:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tPassword: ");
                                user.Password = Console.ReadLine() ?? string.Empty;
                                while (!Validator.PasswordValidate(user.Password, user.Id, users))
                                {
                                    if (string.IsNullOrEmpty(user.Password))
                                        Console.WriteLine("\tPassword cannot be empty!");
                                    else
                                        Console.WriteLine("\tPassword is used!");
                                    Console.Write("\tPassword: ");
                                    user.Password = Console.ReadLine() ?? string.Empty;
                                }
                                Console.Write("\n\tEnter new password again: ");
                                while (user.Password != (Console.ReadLine() ?? string.Empty))
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.WriteLine("\tPasswords not matched");
                                    Console.WriteLine("\n\tPress \"Esc\" for cancel or any other key to continue...");
                                    ConsoleKeyInfo userInput = Console.ReadKey();
                                    if (userInput.Key == ConsoleKey.Escape)
                                    {
                                        cancel = true;
                                        running = false;
                                        break;
                                    }
                                    else
                                    {
                                        menuHelper.PrintAppName();
                                        Console.WriteLine(title);
                                        Console.Write("\tEnter new password again: ");
                                    }
                                }
                                break;
                            case 4:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tEmail: ");
                                user.Email = Console.ReadLine() ?? string.Empty;
                                while (!Validator.EmailValidate(user.Email, user.Id, users))
                                {
                                    if (string.IsNullOrEmpty(user.Email))
                                        Console.WriteLine("\tEmail address cannot be empty!");
                                    else
                                        Console.WriteLine("\tEmail address is used or not valid!");
                                    Console.Write("\tEmail: ");
                                    user.Email = Console.ReadLine() ?? string.Empty;
                                }
                                break;
                            case 5:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tPhone number: ");
                                user.Phone = Console.ReadLine() ?? string.Empty;
                                while (string.IsNullOrEmpty(user.Phone))
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.WriteLine("\tPhone number is empty! Continue? (\"Y/n\"): ");
                                    if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                        break;
                                    else
                                    {
                                        menuHelper.PrintAppName();
                                        Console.WriteLine(title);
                                        Console.Write("\tPhone number: ");
                                        user.Phone = Console.ReadLine() ?? string.Empty;
                                    }
                                }
                                break;
                            case 6:
                                menuHelper.PrintAppName();
                                Console.WriteLine(title);
                                Console.Write("\tAddress: ");
                                user.Address = Console.ReadLine() ?? string.Empty;
                                while (string.IsNullOrEmpty(user.Address))
                                {
                                    menuHelper.PrintAppName();
                                    Console.WriteLine(title);
                                    Console.WriteLine("\tAddress is empty! Continue? (\"Y/n\"): ");
                                    if ((Console.ReadLine() ?? "n").ToLower() == "y")
                                        break;
                                    else
                                    {
                                        menuHelper.PrintAppName();
                                        Console.WriteLine(title);
                                        Console.Write("\tAddress number: ");
                                        user.Address = Console.ReadLine() ?? string.Empty;
                                    }
                                }
                                break;
                            case 7:
                                if (isAdmin && !(admin != null && !(admin.CompareTo(user) != 0)))
                                {
                                    user.IsAdmin = !user.IsAdmin;
                                }
                                break;
                            case 8:
                                if (isAdmin && !(admin != null && !(admin.CompareTo(user) != 0)))
                                {
                                    user.Deactivated = !user.Deactivated;
                                }
                                break;
                            case 9:
                                if (string.IsNullOrWhiteSpace(user.Name))
                                {
                                    Console.WriteLine("\tName of uesr cannot be empty. Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                                }
                                if (string.IsNullOrWhiteSpace(user.Email))
                                {
                                    Console.WriteLine("\tEmail cannot be empty. Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                                }
                                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                                {
                                    Console.WriteLine("\tInvalid Username or Password. Press any key to continue...");
                                    Console.ReadKey();
                                    break;
                                }
                                running = false;
                                break;
                            case 10:
                                cancel = true;
                                running = false;
                                break;
                        }

                        Console.CursorVisible = false;
                        menuHelper.PrintAppName();
                        Console.WriteLine(title);
                        (menuParams.left, menuParams.top) = Console.GetCursorPosition();

                        break;

                }
            }

            if (cancel)
                return false;
            else
            {
                if (addNew)
                    DataHelper.InsertUsers([user]);
                else
                {
                    int index = users.FindIndex(u => u.Id == user.Id);
                    if (index == -1)
                        users.Add(user);
                    else
                        users[index] = user;
                    DataHelper.UpdateUsers(users);
                }
            }

            return true;
        }

        public static void DeleteUser(User user)
        {
            Console.Clear();
            Console.Write($"\tDelete user \"{user.Name}\" and all data for the user? (\"Y/n\"): ");
            if ((Console.ReadLine() ?? "n").ToLower() != "y")
                return;

            List<User> users = DataHelper.GetUserList();
            int index = users.FindIndex(u => u.Id == user.Id);
            if (index == -1)
                return;

            users.RemoveAt(index);
            DataHelper.UpdateUsers(users);

        }

        public static void PrintUsers(User? admin = null)
        {
            (new MenuHelper()).PrintAppName();
            Console.WriteLine("\t\tUsers\n");

            List<User> users = DataHelper.GetUserList();
            if (admin  != null)
                users = users.Where(u => u.Id != ((User)admin).Id).ToList();

            int counter = 0;
            foreach (User u in users)
                Console.WriteLine($"\t{++counter}. {u.ShortInfo()}");

            Console.WriteLine("\n\tPress any key to continue...");
            Console.ReadKey();
        }

        public User? SelectUser(User? admin, User? user)
        {
            List<User> users = new List<User>();

            if (admin != null && admin.IsAdmin)
                users = DataHelper.GetUserList();
            else if (admin != null)
                users.Add(admin);

            if (user != null && (admin != null && admin.IsAdmin))
                users = users.Where(u => u.Id != ((User)user).Id).ToList();

            if (users.Count == 0)
                return user;

            Console.CursorVisible = false;
            MenuHelper menuHelper = new MenuHelper();
            menuHelper.PrintAppName();
            Console.WriteLine("\t\tUsers\n");

            var menuParams = new MenuHelper.MenuParams();
            (menuParams.left, menuParams.top) = Console.GetCursorPosition();

            Func<string[], string[]> iName = (string[] n) => n;
            Dictionary<int, string[]> menu = users.Select((val, index) => new { Index = index, Value = val })
                                                    .ToDictionary(h => h.Index, h => iName([h.Value.Name, h.Value.Id.ToString()]));
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
                            user = GetUserById(users, int.Parse(menu[menuParams.choice][1]));
                        running = false;
                        break;
                }
            }

            return user;
        }

        

        // TODO - Encrypt, Decript password

    }

}
