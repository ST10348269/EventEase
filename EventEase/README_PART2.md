# EventEase — Part 2: Enhanced Functionality & Cloud Storage

## What Was Implemented

### A. Azure Blob Storage Integration
- Created `AzureBlobService` (`Services/AzureBlobService.cs`) using `Azure.Storage.Blobs` SDK
- **Venues**: Now accept `IFormFile` image uploads instead of raw URLs. Images are uploaded to Azure Blob container `eventease-images` and the public URL is stored in the database
- **Events**: Same image upload to Azure Blob Storage
- Old images are deleted from Azure Blob when replaced
- `appsettings.json` has `AzureStorage:ConnectionString` and `AzureStorage:ContainerName` settings

**To configure Azure Storage:**
1. Create an Azure Storage Account in your Azure portal
2. Create a container named `eventease-images` (set to Blob public access)
3. Copy the connection string to `appsettings.json` → `AzureStorage:ConnectionString`

### B. Error Handling & Validation
- **Double Booking Prevention**: Creating or editing a booking checks if the same venue is already booked on the same date — returns a clear error if so
- **Delete Protection — Venues**: Cannot delete a venue that has active bookings or is linked to events
- **Delete Protection — Events**: Cannot delete an event that has active bookings
- **Required field validation** on all models with `[Required]` attributes
- **Alert display function**: `ShowAlert(type, message)` in controllers; TempData-based alerts shown in all views
- Forms use `asp-validation-summary` and `asp-validation-for` for client-side + server-side validation

### C. Enhanced Display & Search
- New `BookingDetailsView` model (`Models/BookingDetailsView.cs`) mirrors SQL view `vw_BookingDetails`
- `vw_BookingDetails` consolidates Bookings + Events + Venues into one rich record
- Bookings Index now uses this view to display: event image, event name/description, event date, venue image, venue name, location, capacity, booking date, status
- **Search**: Bookings can be searched by **Booking ID** or **Event Name** via `?searchQuery=` parameter

### D. Database Migration
Two options to apply changes:

**Option 1 — EF Core Migration** (recommended for code-first):
```bash
dotnet ef database update
```
This runs migration `20260508000000_Part2AddEventImageUrl` which:
- Adds `ImageUrl` column to `Events` table
- Makes `Venues.ImageUrl` nullable
- Creates `vw_BookingDetails` SQL view

**Option 2 — Manual SQL Script**:
Run `SQL_Migration_Part2.sql` directly in Azure Data Studio or SSMS against your database.

## Project Structure Changes
```
EventEase/
├── Services/
│   └── AzureBlobService.cs          ← NEW: Azure Blob upload/delete
├── Models/
│   ├── Venue.cs                     ← UPDATED: validation + IFormFile
│   ├── Event.cs                     ← UPDATED: validation + IFormFile + ImageUrl
│   ├── Booking.cs                   ← UPDATED: Required attributes
│   └── BookingDetailsView.cs        ← NEW: keyless ViewModel for vw_BookingDetails
├── Controllers/
│   ├── VenuesController.cs          ← UPDATED: blob upload, delete protection
│   ├── EventsController.cs          ← UPDATED: blob upload, delete protection
│   └── BookingsController.cs        ← UPDATED: double-booking check, search, view
├── Data/
│   └── ApplicationDbContext.cs      ← UPDATED: BookingDetailsView keyless entity
├── Migrations/
│   └── 20260508000000_Part2...cs    ← NEW: EF migration
└── Views/
    ├── Venues/Create.cshtml          ← UPDATED: file upload
    ├── Venues/Edit.cshtml            ← UPDATED: file upload + current image
    ├── Venues/Index.cshtml           ← UPDATED: blob images + alerts
    ├── Events/Create.cshtml          ← UPDATED: file upload
    ├── Events/Edit.cshtml            ← UPDATED: file upload + current image
    └── Bookings/Index.cshtml         ← UPDATED: enhanced table + search bar
```
