
using System.Net.Mail;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem.Helpers
{
    internal class Validator
    {
        public static bool EmailValidate(string email, int id, List<User>? list = null)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                MailAddress m = new MailAddress(email);
            }
            catch (Exception)
            {
                return false;
            }

            if (list == null)
                list = DataHelper.GetUserList();

            if (list != null && list.Where(u => u.Email == email && u.Id != id).Count() > 0)
                return false;

            return true;
        }

        public static bool NameValidate(string name, int id, List<Hotel>? list = null)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (list == null)
                list = DataHelper.GetHotelList();

            if (list != null && list.Where(h => h.Name == name && h.Id != id).Count() > 0)
                return false;

            return true;
        }
        
        public static bool NameValidate(string name, int id, List<User>? list = null)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (list == null)
                list = DataHelper.GetUserList();

            if (list != null && list.Where(u => u.Name == name && u.Id != id).Count() > 0)
                return false;

            return true;
        }

        public static bool NameValidate(string name, int id, int hotelId, List<RoomType>? list = null)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (list == null)
                list = DataHelper.GetRoomTypeList();

            if (list != null && list.Where(r => r.Name == name && r.Id != id && r.HotelId == hotelId).Count() > 0)
                return false;

            return true;
        }

        public static bool RoomNumberValidate(int number, int id, int hotelId, List<Room>? list = null)
        {
            if (number == 0)
                return false;

            if (list == null)
                list = RoomHelper.GetRooms([hotelId]);

            if (list != null && list.Where(r => r.Number == number && r.Id != id && r.HotelId == hotelId).Count() > 0)
                return false;

            return true;
        }

        public static bool UsernameValidate(string username, int id, List<User>? list = null)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            if (list == null)
                list = DataHelper.GetUserList();

            if (list != null && list.Where(u => u.Username == username && u.Id != id).Count() > 0)
                return false;

            return true;
        }

        public static bool PasswordValidate(string password, int id, List<User>? list = null)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (list == null)
                list = DataHelper.GetUserList();

            // TODO - check password strength

            return true;
        }

    }
}
