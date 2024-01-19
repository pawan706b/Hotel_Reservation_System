using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Reservation_System.Models
{
    public class Billing
    {
        [Key]
        public int InvoiceID { get; set; }
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        [Required]
        public string PaymentMethod { get; set; }

        // Navigation property
        //public virtual Reservation Reservation { get; set; }
    }

}
