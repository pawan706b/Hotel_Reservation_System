using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Data;
using Hotel_Reservation_System.Models;
using Microsoft.AspNetCore.Authorization;

namespace Hotel_Reservation_System.Controllers
{
    public class RoomsController : Controller
    {
        private readonly Hotel_Reservation_SystemContext _context;

        public RoomsController(Hotel_Reservation_SystemContext context)
        {
            _context = context;
        }

        // GET: Rooms/ViewRooms/{hotelId}
        public async Task<IActionResult> Index(int? hotelId)
        {
            List<Room> roomsForHotel;

            if (hotelId.HasValue)
            {
                roomsForHotel = await _context.Room
                    .Where(r => r.HotelID == hotelId.Value)
                    .ToListAsync();
            }
            else
            {
                roomsForHotel = await _context.Room.ToListAsync();
            }

            // You may want to remove this check or modify it according to your requirements
            if (!roomsForHotel.Any())
            {
                ModelState.AddModelError(string.Empty, "No rooms available for this hotel");
            }
            return View(roomsForHotel);
        }



        public async Task<IActionResult> FilteredRooms(DateTime? checkin, DateTime? checkout, int? capacity)
        {

            // Get available rooms for the given date range and capacity asynchronously
            // Validate input
            if (checkin >= checkout)
            {
                ModelState.AddModelError(string.Empty, "Check-out date must be after check-in date.");
                return View();
            }
            var availableRooms = await GetAvailableRoomsAsync(checkin, checkout, capacity);

            // Pass the available rooms, dates, and capacity to the view
            ViewBag.Checkin = checkin;
            ViewBag.Checkout = checkout;
            ViewBag.Capacity = capacity;

            return View(availableRooms);
        }


        private async Task<List<Room>> GetAvailableRoomsAsync(DateTime? checkInDate, DateTime? checkOutDate, int? requiredCapacity)
        {
            var reservedRoomIds = await _context.Reservation
                .Where(r => (r.CheckInTime < checkOutDate && r.CheckOutTime > checkInDate))
                .Select(r => r.RoomID)
                .ToListAsync();

            var availableRooms = await _context.Room
                .Where(r => !reservedRoomIds.Contains(r.RoomID) && r.Capacity == requiredCapacity)
                .ToListAsync();

            return availableRooms;
        }


        // GET: Rooms/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Room == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("RoomID,HotelID,RoomType,BedType,Capacity,Status,Rate")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Room == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("RoomID,HotelID,RoomType,BedType,Capacity,Status,Rate")] Room room)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // GET: Rooms/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Room == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Room == null)
            {
                return Problem("Entity set 'Hotel_Reservation_SystemContext.Room'  is null.");
            }
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        private bool RoomExists(int id)
        {
          return (_context.Room?.Any(e => e.RoomID == id)).GetValueOrDefault();
        }
    }
}
