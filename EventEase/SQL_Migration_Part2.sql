


IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Events' AND COLUMN_NAME = 'ImageUrl'
)
BEGIN
    ALTER TABLE Events ADD ImageUrl NVARCHAR(500) NULL;
    PRINT 'Added ImageUrl column to Events table.';
END
ELSE
BEGIN
    PRINT 'ImageUrl column already exists in Events table.';
END
GO


IF EXISTS (SELECT 1 FROM sys.views WHERE name = 'vw_BookingDetails')
BEGIN
    DROP VIEW vw_BookingDetails;
    PRINT 'Dropped existing vw_BookingDetails view.';
END
GO


CREATE VIEW vw_BookingDetails AS
SELECT
    b.BookingID,
    b.BookingDate,
    e.EventName,
    e.EventDate,
    e.Description          AS EventDescription,
    e.ImageUrl             AS EventImageUrl,
    v.VenueName,
    v.Location             AS VenueLocation,
    v.Capacity             AS VenueCapacity,
    v.ImageUrl             AS VenueImageUrl
FROM
    Bookings b
    INNER JOIN Events  e ON b.EventID  = e.EventID
    INNER JOIN Venues  v ON b.VenueID  = v.VenueID;
GO

PRINT 'Successfully created vw_BookingDetails view.';


SELECT TOP 5 * FROM vw_BookingDetails;
GO
