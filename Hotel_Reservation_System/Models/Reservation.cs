using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Reservation_System.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        [ForeignKey("Room")]
        public int RoomID { get; set; }
        [Required]
        public DateTime CheckInTime { get; set; }
        [Required]
        public DateTime CheckOutTime { get; set; }
        [Required]
        public string Status { get; set; }

        // Navigation properties
        //public virtual User User { get; set; }
        //public virtual Room Room { get; set; }
        public virtual Billing Billing { get; set; }
    }

}
