CREATE PROCEDURE [dbo].[spBookings_Insert]
	@roomId int,
	@guestId int,
	@startDate date,
	@endDate date,
	@totalCost money
AS
begin
	set nocount on;

	INSERT INTO dbo.Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
	VALUES(@roomId, @guestId, @startDate, @endDate, @totalCost);
end
