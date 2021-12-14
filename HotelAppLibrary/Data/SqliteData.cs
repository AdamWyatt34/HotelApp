using HotelAppLibrary.Database;
using HotelAppLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelAppLibrary.Data
{
    public class SqliteData : IDatabaseData
    {
        private const string connectionStringName = "SQLiteDb";
        private readonly ISqliteDataAccess _db;

        public SqliteData(ISqliteDataAccess db)
        {
            _db = db;
        }

        public void BookGuest(string firstName, string lastName, DateTime startDate, DateTime endDate, int roomTypeId)
        {
            string sql = @"SELECT 1 FROM Guests WHERE FirstName = @firstName AND LastName = @lastName";

            int results = _db.LoadData<dynamic,dynamic>(sql, new {firstName, lastName}, connectionStringName).Count;

            if (results > 0)
            {
                sql = @"INSERT INTO Guests(FirstName, LastName)
		                        VALUES(@firstName, @lastName)";
                _db.SaveData(sql, new { firstName, lastName }, connectionStringName);
            }

            sql = @"SELECT [Id], [FirstName], [LastName]
	                        FROM Guests
	                        WHERE FirstName = @firstName AND LastName = @lastName LIMIT 1;";

            GuestModel guest = _db.LoadData<GuestModel, dynamic>(sql,
                                                                 new { firstName, lastName },
                                                                 connectionStringName).First();

            RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>("SELECT * FROM RoomTypes WHERE Id = @id",
                                                                          new { id = roomTypeId },
                                                                          connectionStringName).First();

            TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);

            sql = @"select r.*
	                from Rooms r
	                inner join RoomTypes t ON t.Id = r.RoomTypeId
	                WHERE r.RoomTypeId = @roomTypeId
	                and
	                r.Id not in(
	                select b.RoomId
	                from Bookings b
	                where (@startDate < b.StartDate and @endDate > b.EndDate)
		                or (b.StartDate <= @endDate and @endDate < b.EndDate)
		                or (b.StartDate <= @startDate and @startDate < b.EndDate)
	                )";

            List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>(sql,
                                                                              new { startDate, endDate, roomTypeId },
                                                                              connectionStringName);

            sql = @"INSERT INTO Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
	                VALUES(@roomId, @guestId, @startDate, @endDate, @totalCost);";

            _db.SaveData(sql,
                new
                {
                    roomId = availableRooms.First().Id,
                    guestId = guest.Id,
                    startDate = startDate,
                    endDate = endDate,
                    totalCost = timeStaying.Days * roomType.Price
                },
                connectionStringName);
        }

        public void CheckInGuest(int bookingId)
        {
            string sql = @"UPDATE Bookings 
	                        SET CheckedIn = 1
	                        WHERE Id = @Id;";

            _db.SaveData(sql, new { Id = bookingId }, connectionStringName);
        }

        public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
        {
            string sql = @"select t.Id, t.Title, t.Description, t.Price
	                        from Rooms r
	                        inner join RoomTypes t ON t.Id = r.RoomTypeId
	                        WHERE r.Id not in(
	                        select b.RoomId
	                        from Bookings b
	                        where (@startDate < b.StartDate and @endDate > b.EndDate)
		                        or (b.StartDate <= @endDate and @endDate < b.EndDate)
		                        or (b.StartDate <= @startDate and @startDate < b.EndDate)
	                        )
	                        GROUP BY t.Id, t.Title, t.Description, t.Price;";


            var output = _db.LoadData<RoomTypeModel, dynamic>(sql,
                                                 new { startDate, endDate },
                                                 connectionStringName);

            output.ForEach(x => x.Price = x.Price / 100);


            return output;
        }

        public RoomTypeModel GetRoomTypeById(int id)
        {
            string sql = @"select [Id], [Title], [Description], [Price]
	                        from RoomTypes
	                        where Id = @Id;";

            return _db.LoadData<RoomTypeModel, dynamic>(sql,
                                                       new { id },
                                                       connectionStringName).FirstOrDefault();
        }

        public List<BookingFullModel> SearchBookings(string lastName)
        {
            string sql = @"SELECT [b].[Id], [b].[RoomId], [b].[GuestId], [b].[StartDate], [b].[EndDate], 
	                        [b].[CheckedIn], [b].[TotalCost], [g].[FirstName], [g].[LastName], [r].[RoomNumber], [r].[RoomTypeId],
	                        [t].[Title], [t].[Description], [t].[Price]
	                        FROM Bookings b
	                        INNER JOIN Guests g ON b.GuestId = g.Id
	                        INNER JOIN Rooms r ON b.RoomId = r.Id
	                        INNER JOIN RoomTypes t ON r.RoomTypeId = t.Id
	                        WHERE b.StartDate = @startDate AND g.LastName = @lastName;";


            var output = _db.LoadData<BookingFullModel, dynamic>(sql,
                                                    new { lastName, startDate = DateTime.Now.Date },
                                                    connectionStringName);

            output.ForEach(x =>
            {
                x.Price = x.Price / 100;
                x.TotalCost = x.TotalCost / 100;
            });

            return output;
        }
    }
}
