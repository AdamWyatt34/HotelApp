CREATE PROCEDURE [dbo].[spGuests_Insert]
	@firstName nvarchar(50),
	@lastName nvarchar(50)
AS
begin
	set nocount on;
	
	if not exists(SELECT 1 FROM dbo.Guests WHERE FirstName = @firstName AND LastName = @lastName)
	begin
		INSERT INTO dbo.Guests(FirstName, LastName)
		VALUES(@firstName, @lastName)
	end

	SELECT top 1 [Id], [FirstName], [LastName]
	FROM dbo.Guests
	WHERE FirstName = @firstName AND LastName = @lastName;


end
