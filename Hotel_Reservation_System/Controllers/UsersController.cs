using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hotel_Reservation_System.Data;
using Hotel_Reservation_System.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Hotel_Reservation_System.Controllers
{
    public class UsersController : Controller
    {
        private readonly Hotel_Reservation_SystemContext _context;

        public UsersController(Hotel_Reservation_SystemContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.User != null ? 
                          View(await _context.User.ToListAsync()) :
                          Problem("Entity set 'Hotel_Reservation_SystemContext.User'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserID,UserName,Password,Email,Role")] User user)
        {
            if (user.UserName == null || user.Password == null || user.Email == null)
            {
                ModelState.AddModelError("", "User data is required.");
                return View(user);
            }
            // Password validation
            bool hasUpperCase = user.Password.Any(char.IsUpper);
            bool hasLowerCase = user.Password.Any(char.IsLower);
            bool hasDigits = user.Password.Any(char.IsDigit);
            bool isLengthValid = user.Password.Length > 8;


            // Email validation
            // Username existence check
            if (_context.User.Any(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "This username already exists.");
            }

            else if (_context.User.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "This Email already exists.");
            }

            else if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format.");
            }

            else if (!hasUpperCase || !hasLowerCase || !hasDigits || !isLengthValid)
            {
                ModelState.AddModelError("Password", "Password must be longer than 8 characters and contain uppercase, lowercase letters, and at least one digit.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login", "Users");
            }
            return View(user);
        }


        // GET: Users/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,UserName,Password,Email,Role")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
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
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Hotel_Reservation_SystemContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Users/Create
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password")] User user)
        {
            var authenticatedUser = await _context.User
                .FirstOrDefaultAsync(u => u.UserName == user.UserName && u.Password == user.Password);

            if (authenticatedUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, authenticatedUser.UserID.ToString()),
                    new Claim(ClaimTypes.Name, authenticatedUser.UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true, // For a persistent cookie, or false for a session cookie
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Set expiry time
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Add a model error if the user authentication fails
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            // Return the same view with user data if authentication fails
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Clearing the session
            HttpContext.Session.Clear();

            // Sign out the user from the authentication middleware
            await HttpContext.SignOutAsync("CookieAuth");

            // Redirect to the Login page or any other page you see fit
            return RedirectToAction("Login", "Users");
        }

        private bool UserExists(int id)
        {
          return (_context.User?.Any(e => e.UserID == id)).GetValueOrDefault();
        }

        [Authorize]
        public async Task<IActionResult> Service()
        {
            // Your logic here, if any (e.g., preparing a model to pass to the view)

            return View();
        }
    }
}
