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
    public class BillingsController : Controller
    {
        private readonly Hotel_Reservation_SystemContext _context;

        public BillingsController(Hotel_Reservation_SystemContext context)
        {
            _context = context;
        }

        // GET: Billings
        [Authorize]
        public async Task<IActionResult> Index()
        {
              return _context.Billing != null ? 
                          View(await _context.Billing.ToListAsync()) :
                          Problem("Entity set 'Hotel_Reservation_SystemContext.Billing'  is null.");
        }

        // GET: Billings/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Billing == null)
            {
                return NotFound();
            }

            var billing = await _context.Billing
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (billing == null)
            {
                return NotFound();
            }

            return View(billing);
        }

        // GET: Billings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Billings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("InvoiceID,ReservationID,TotalAmount,PaymentMethod")] Billing billing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billing);
                var reservation = await _context.Reservation.FindAsync(billing.ReservationID);
                if (reservation != null)
                {
                    // Update the reservation status
                    reservation.Status = "confirmed";
                    _context.Update(reservation);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Reservations");
            }
            return RedirectToAction("Login", "Users");
        }

        // GET: Billings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Billing == null)
            {
                return NotFound();
            }

            var billing = await _context.Billing.FindAsync(id);
            if (billing == null)
            {
                return NotFound();
            }
            return View(billing);
        }

        // POST: Billings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceID,ReservationID,TotalAmount,PaymentMethod")] Billing billing)
        {
            if (id != billing.InvoiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillingExists(billing.InvoiceID))
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
            return View(billing);
        }

        // GET: Billings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Billing == null)
            {
                return NotFound();
            }

            var billing = await _context.Billing
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (billing == null)
            {
                return NotFound();
            }

            return View(billing);
        }

        // POST: Billings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Billing == null)
            {
                return Problem("Entity set 'Hotel_Reservation_SystemContext.Billing'  is null.");
            }
            var billing = await _context.Billing.FindAsync(id);
            if (billing != null)
            {
                _context.Billing.Remove(billing);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillingExists(int id)
        {
          return (_context.Billing?.Any(e => e.InvoiceID == id)).GetValueOrDefault();
        }
    }
}
