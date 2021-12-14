CREATE PROCEDURE [dbo].[spBookings_CheckIn]
	@Id int
AS
begin
	set nocount on;

	UPDATE dbo.Bookings 
	SET CheckedIn = 1
	WHERE Id = @Id;
end

