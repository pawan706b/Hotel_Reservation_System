using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Reservation_System.Models
{
    public class Manager
    {
        [Key]
        public int ManagerID { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }

        // Navigation property
        //public virtual User User { get; set; }
    }

}
