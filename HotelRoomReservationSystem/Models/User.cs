﻿namespace HotelRoomReservationSystem.Models
{
    public class User : BaseModel
    {
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

        public override string Info()
        {
            return $"\tID: {Id}\n" + 
                   $"\tName: {Name}\n" +
                   $"\temail: {Email}\n" +
                   $"\tUsername: {Username}\n" +
                   $"\tPassword: ****\n" +
                   $"\tPhone: {Phone}\n" +
                   $"\tAddress: {Address}\n" +
                   $"\tAdministrator: {(IsAdmin ? "yes" : "no")}\n" +
                   $"\tDeactivated: {(Deactivated ? "yes" : "no" )}";
        }

        public override string ShortInfo()
        {
            return $"Name {Name}, id {Id}" + ((IsAdmin) ? " (admin)" : "");
        }
    }
}
