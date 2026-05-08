using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings — Enhanced view with search using vw_BookingDetails
        public async Task<IActionResult> Index(string? searchQuery)
        {
            ViewData["SearchQuery"] = searchQuery;

            IQueryable<BookingDetailsView> query;

            try
            {
                // Use the SQL View for enhanced display
                query = _context.BookingDetailsViews;

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    var lower = searchQuery.ToLower();
                    query = query.Where(b =>
                        b.BookingID.ToString().Contains(lower) ||
                        b.EventName.ToLower().Contains(lower));
                }

                var results = await query.ToListAsync();
                return View(results);
            }
            catch
            {
                // Fallback if view doesn't exist yet — show empty with message
                TempData["ErrorMessage"] = "The booking details view (vw_BookingDetails) is not yet created in the database. Please run the SQL migration script.";
                return View(new List<BookingDetailsView>());
            }
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName");
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,BookingDate,EventID,VenueID")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // VALIDATION: Prevent double booking — same venue, same date/time
                bool isDuplicateBooking = await _context.Bookings.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingDate.Date == booking.BookingDate.Date &&
                    b.BookingID != booking.BookingID);

                if (isDuplicateBooking)
                {
                    ModelState.AddModelError("", "This venue is already booked for a different event on the selected date. Please choose a different date or venue.");
                    ShowAlert("warning", "Double booking detected. This venue is already booked on that date.");
                    ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
                    ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking was created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    ShowAlert("danger", "Failed to create booking. Please try again.");
                }
            }
            else
            {
                ShowAlert("danger", "Please fix the validation errors below.");
            }

            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,BookingDate,EventID,VenueID")] Booking booking)
        {
            if (id != booking.BookingID) return NotFound();

            if (ModelState.IsValid)
            {
                // VALIDATION: Prevent double booking on edit
                bool isDuplicateBooking = await _context.Bookings.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingDate.Date == booking.BookingDate.Date &&
                    b.BookingID != booking.BookingID);

                if (isDuplicateBooking)
                {
                    ModelState.AddModelError("", "This venue is already booked for a different event on the selected date.");
                    ShowAlert("warning", "Double booking detected. Choose a different date or venue.");
                    ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
                    ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking was updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            else
            {
                ShowAlert("danger", "Please fix the validation errors below.");
            }

            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking was deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }

        private void ShowAlert(string type, string message)
        {
            TempData["AlertType"] = type;
            TempData["AlertMessage"] = message;
        }
    }
}
