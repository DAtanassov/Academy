
using HotelRoomReservationSystem.Models.Interfaces;

namespace HotelRoomReservationSystem.Models
{
    public abstract class BaseModel : IModel, IComparable<BaseModel>
    {
        public int Id { get; set; }

        public int CompareTo(BaseModel? other)
        {
            if (other == null) return 1;
            return this.Id.CompareTo(other.Id);
        }

        public abstract string Info();
        public abstract string ShortInfo();


    }
}
