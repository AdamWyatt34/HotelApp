CREATE PROCEDURE [dbo].[spRoomTypes_GetAvailableTypes]
	@startDate date,
	@endDate date
AS
begin
	set nocount on;

	select t.Id, t.Title, t.Description, t.Price
	from dbo.Rooms r
	inner join dbo.RoomTypes t ON t.Id = r.RoomTypeId
	WHERE r.Id not in(
	select b.RoomId
	from dbo.Bookings b
	where (@startDate < b.StartDate and @endDate > b.EndDate)
		or (b.StartDate <= @endDate and @endDate < b.EndDate)
		or (b.StartDate <= @startDate and @startDate < b.EndDate)
	)
	GROUP BY t.Id, t.Title, t.Description, t.Price

end
