using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureBlobService _blobService;
        private const string ContainerName = "eventease-images";

        public VenuesController(ApplicationDbContext context, IAzureBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueID,VenueName,Location,Capacity")] Venue venue, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Upload image to Azure Blob Storage
                        venue.ImageUrl = await _blobService.UploadImageAsync(ImageFile, ContainerName);
                    }

                    _context.Add(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Venue '{venue.VenueName}' was created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving the venue: {ex.Message}");
                    ShowAlert("danger", "Failed to create venue. Please try again.");
                }
            }
            else
            {
                ShowAlert("danger", "Please fix the validation errors below.");
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueID,VenueName,Location,Capacity,ImageUrl")] Venue venue, IFormFile? ImageFile)
        {
            if (id != venue.VenueID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Delete old image from blob storage if it exists
                        if (!string.IsNullOrEmpty(venue.ImageUrl))
                        {
                            await _blobService.DeleteImageAsync(venue.ImageUrl, ContainerName);
                        }
                        // Upload new image
                        venue.ImageUrl = await _blobService.UploadImageAsync(ImageFile, ContainerName);
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Venue '{venue.VenueName}' was updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueID)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    ShowAlert("danger", "Failed to update venue. Please try again.");
                }
            }
            else
            {
                ShowAlert("danger", "Please fix the validation errors below.");
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueID == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // VALIDATION: Prevent deletion if venue has active bookings
            bool hasActiveBookings = await _context.Bookings.AnyAsync(b => b.VenueID == id);
            if (hasActiveBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it has active bookings. Please remove the associated bookings first.";
                ShowAlert("danger", "Cannot delete venue with active bookings.");
                return RedirectToAction(nameof(Index));
            }

            // VALIDATION: Prevent deletion if venue is linked to events
            bool hasEvents = await _context.Events.AnyAsync(e => e.VenueID == id);
            if (hasEvents)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it is associated with existing events. Please remove or reassign those events first.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                // Delete image from Azure Blob if it exists
                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    await _blobService.DeleteImageAsync(venue.ImageUrl, ContainerName);
                }

                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Venue '{venue.VenueName}' was deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueID == id);
        }

        private void ShowAlert(string type, string message)
        {
            TempData["AlertType"] = type;
            TempData["AlertMessage"] = message;
        }
    }
}
