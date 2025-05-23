namespace HotelRoomReservationSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public bool IsAdmin { get; set; }
        public bool Deactivated { get; set; }

        public User() { }
        public User(string name, string email, string username, string password, bool isAdmin)
        {
            this.Name = name;
            this.Password = password;
            this.Username = username;
            this.Email = email.ToLower().Trim();
            this.Phone = "";
            this.Address = "";
            this.IsAdmin = isAdmin;
            this.Deactivated = false;
        }

        public string GetInfo(User user)
        {
            if (!(user.Id == Id || user.IsAdmin))
                return "";

            return $"\tID: {Id}\n" + 
                   $"\tName: {Name}\n" +
                   $"\te-mail: {Email}\n" +
                   $"\tUsername: {Username}\n" +
                   $"\tPassword: {Password}\n" +
                   $"\tPhone: {Phone}\n" +
                   $"\tAddress: {Address}\n" +
                   $"\tAdministrator: {((IsAdmin) ? "yes" : "no")}\n" +
                   $"\tDeactivated: {((Deactivated) ? "yes" : "no" )}";
        }

        public string GetShortInfo(bool showAdminStatus = false)
        {
            return $"Name {Name}, id {Id}" + ((showAdminStatus && IsAdmin) ? " (administrator)" : "");
        }
    }
}
