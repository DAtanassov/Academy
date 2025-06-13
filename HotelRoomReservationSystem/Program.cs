using System.Text;
using HotelRoomReservationSystem.Helpers;
using HotelRoomReservationSystem.Models;

namespace HotelRoomReservationSystem
{
    internal class Program
    {
        public static User? user;

        static void Main(string[] args)
        {
            Console.Clear();
            Console.OutputEncoding = Encoding.UTF8;

            ReservationHelper.CheckAndCancelExpiredReservations();

            (new Menu()).Run();

            Environment.Exit(0);
        }
        
    }
}
