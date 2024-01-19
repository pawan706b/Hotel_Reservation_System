using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Data;
using Hotel_Reservation_System.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Hotel_Reservation_System.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly Hotel_Reservation_SystemContext _context;

        public ReservationsController(Hotel_Reservation_SystemContext context)
        {
            _context = context;
        }

        // GET: Reservations
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = int.Parse(userIdClaim.Value);

            var reservationRoomData = await _context.Reservation
                .Where(reservation => reservation.UserID == userId)
                .Join(_context.Room,
                    reservation => reservation.RoomID,
                    room => room.RoomID,
                    (reservation, room) => new { Reservation = reservation, Room = room })
                .ToListAsync(); // Execute the query and get the result in memory

            var userReservations = reservationRoomData
                .Join(_context.Billing,
                    combined => combined.Reservation.ReservationID,
                    billing => billing.ReservationID,
                    (combined, billing) => (Reservation: combined.Reservation, Room: combined.Room, Billing: billing)) // Now we're operating in memory, not in SQL
                .ToList();
            Console.WriteLine(userReservations);
            return View(userReservations);
        }




        // GET: Booking/Payment
        [Authorize]
        public async Task<IActionResult> Payment(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(reservation.RoomID);
            if (room == null)
            {
                return NotFound();
            }

            // Calculate the difference in days between check-out and check-in
            int days = (reservation.CheckOutTime - reservation.CheckInTime).Days;

            ViewBag.ReservationID = id;
            ViewBag.TotalAmount = room.Rate * days;

            return View();
        }


        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ReservationID,UserID,RoomID,CheckInTime,CheckOutTime,Status")] Reservation reservation)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                reservation.UserID = int.Parse(userIdClaim.Value);
            }
            reservation.Status = "pending";
            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Payment", "Reservations", new { id = reservation.ReservationID });
        }

        // GET: Reservations/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationID,UserID,RoomID,CheckInTime,CheckOutTime,Status")] Reservation reservationUpdate)
        {
            if (id != reservationUpdate.ReservationID)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(reservationUpdate.RoomID);
            if (room == null)
            {
                return NotFound();
            }
            try
            {
                // Update reservation details
                reservation.CheckInTime = reservationUpdate.CheckInTime;
                reservation.CheckOutTime = reservationUpdate.CheckOutTime;
                // Update other fields as necessary

                // Recalculate billing amount
                int days = (reservation.CheckOutTime - reservation.CheckInTime).Days;
                var billing = await _context.Billing.FirstOrDefaultAsync(b => b.ReservationID == id);
                if (billing != null)
                {
                    billing.TotalAmount = room.Rate * days; // Assuming Rate is per day
                    _context.Update(billing);
                }

                // Save changes
                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.ReservationID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index", "Reservations");
        }



        // GET: Reservations/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'Hotel_Reservation_SystemContext.Reservation' is null.");
            }
            var reservation = await _context.Reservation.Include(r => r.Billing).FirstOrDefaultAsync(r => r.ReservationID == id);
            if (reservation != null)
            {
                // Check if there is an associated billing record and delete it
                if (reservation.Billing != null)
                {
                    _context.Billing.Remove(reservation.Billing);
                }

                // Then delete the reservation
                _context.Reservation.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Reservations");
        }


        private bool ReservationExists(int id)
        {
          return (_context.Reservation?.Any(e => e.ReservationID == id)).GetValueOrDefault();
        }
    }
}
