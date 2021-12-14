CREATE PROCEDURE [dbo].[spBookings_Search]
	@lastName nvarchar(50),
	@startDate date
AS

begin

	SELECT [b].[Id], [b].[RoomId], [b].[GuestId], [b].[StartDate], [b].[EndDate], 
	[b].[CheckedIn], [b].[TotalCost], [g].[FirstName], [g].[LastName], [r].[RoomNumber], [r].[RoomTypeId],
	[t].[Title], [t].[Description], [t].[Price]
	FROM dbo.Bookings b
	INNER JOIN dbo.Guests g ON b.GuestId = g.Id
	INNER JOIN dbo.Rooms r ON b.RoomId = r.Id
	INNER JOIN dbo.RoomTypes t ON r.RoomTypeId = t.Id
	WHERE b.StartDate = @startDate AND g.LastName = @lastName;

end
