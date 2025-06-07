using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelRoomReservationSystem.Models
{
    public enum RoomStatus
    { 
        [Description("Available")]
        available = 0,
        [Description("Booked")]
        booked = 1,
        [Description("Occupied")]
        ocupated = 2,
        [Description("Canceled")]
        canceled = 3,
        [Description("Expired booking")]
        expired = 4,
        [Description("Completed booking")]
        completed = 5

    }
}
