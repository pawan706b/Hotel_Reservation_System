using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Reservation_System.Models
{
    public enum UserRole
    {
        User,
        Manager
    }

    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [HiddenInput(DisplayValue = false)]
        public UserRole Role { get; set; } = UserRole.User; // Default role is User

        // Navigation properties
        //public virtual Manager Manager { get; set; }
        //public virtual ICollection<Reservation> Reservations { get; set; }

        public User()
        {
            // Other initializations if necessary
        }
    }
}
