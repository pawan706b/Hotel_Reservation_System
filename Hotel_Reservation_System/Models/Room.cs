using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Reservation_System.Models
{
    public class Room
    {
        [Key]
        public int RoomID { get; set; }
        [ForeignKey("Hotel")]
        public int HotelID { get; set; }
        [Required]
        public string RoomType { get; set; }
        public string BedType { get; set; }
        [Required]
        public int Capacity { get; set; }
        public string Status { get; set; }
        [Required]
        public double Rate { get; set; }

        // Navigation properties
        //public virtual Hotel Hotel { get; set; }
        //public virtual ICollection<Reservation> Reservations { get; set; }
    }

}
