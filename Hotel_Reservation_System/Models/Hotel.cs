using System.ComponentModel.DataAnnotations;

namespace Hotel_Reservation_System.Models
{
    public class Hotel
    {
        [Key]
        public int HotelID { get; set; }
        [Required]
        public string HotelName { get; set; }
        [Required]
        public string Location { get; set; }
        public double DistanceFromCenter { get; set; }
        public double ReviewScore { get; set; }
        public string Description { get; set; }

        // Navigation properties
        //public virtual ICollection<Room> Rooms { get; set; }
    }

}
