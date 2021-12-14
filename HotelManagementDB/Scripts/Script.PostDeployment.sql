/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

If not exists(SELECT 1 FROM dbo.RoomTypes)
begin
    INSERT INTO RoomTypes(Title, Description, Price)
    VALUES('King Size Bed', 'Room with a king size bed and a window.', 100),
    ('Two Queen Size Beds','Room with two queen size beds and a window.', 115),
    ('Executive Suite','Two rooms each with a king size bed and a window.',205)
end

if not exists(Select 1 from dbo.Rooms)
begin
    declare @roomId1 int;
    declare @roomId2 int;
    declare @roomId3 int;

    select @roomId1 = id from dbo.RoomTypes WHERE Title = 'King Size Bed'
    select @roomId2 = id from dbo.RoomTypes WHERE Title = 'Two Queen Size Beds'
    select @roomId3 = id from dbo.RoomTypes WHERE Title = 'Executive Suite'

    INSERT INTO Rooms(RoomNumber, RoomTypeId)
    values('101',@roomId1),
    ('102',@roomId1),
    ('103',@roomId1),
    ('201',@roomId2),
    ('202',@roomId2),
    ('301',@roomId3)
end
