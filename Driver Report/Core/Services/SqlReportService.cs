using Driver_Report.Core.Interface;
using Driver_Report.Core.Models;
using System.Data;
using Dapper;

namespace Driver_Report.Core.Services
{
    public class SqlReportService:IReportService
    {
        private readonly ISqlConnectionFactory _dbFactory;

        public SqlReportService(ISqlConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public void AddReport(Report report)
        {
            using IDbConnection dbConnection = _dbFactory.CreateConnection();
            dbConnection.Open();

            string sql = @"INSERT INTO report (report_reason,report_date,driver_id)
                         VALUES (@ReportReason,@ReportDate,@DriverId)";

            dbConnection.Execute(sql, new
            {
                ReportReason = report.ReportReason,
                ReportDate = DateTime.Now,
                DriverId = report.DriverId
            });

        }

        public int AddDriver(Driver driver)
        {
            using IDbConnection dbConnection = _dbFactory.CreateConnection();
            dbConnection.Open();

            string sql = @"INSERT INTO driver (driver_license_plate, driver_first_name, driver_last_name)
                           VALUES (@DriverLicensePlate, @DriverFirstName, @DriverLastName);
                           
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            return dbConnection.ExecuteScalar<int>(sql, new
            {
                DriverLicensePlate = driver.DriverLicensePlate,
                DriverFirstName = driver.DriverFirstName,
                DriverLastName = driver.DriverLastName
            });
        }


        public Report? GetReport(int reportId)
        {
            using IDbConnection dbConnection = _dbFactory.CreateConnection();
            dbConnection.Open();

            string sql = "SELECT * FROM report WHERE report_id = @reportId";

            return dbConnection.QueryFirstOrDefault<Report>(sql, new { reportId = reportId });
        }

        public Driver? GetDriver(int driverId)
        {
            using IDbConnection dbConnection = _dbFactory.CreateConnection();
            dbConnection.Open();

            string sql = "SELECT * FROM driver WHERE driver_id = @driverId";

            return dbConnection.QueryFirstOrDefault<Driver>(sql, new { driverId = driverId });
        }

        public List<Report> GetDriverReports(int driverId)
        {
            using IDbConnection dbConnection = _dbFactory.CreateConnection();
            dbConnection.Open();

            string sql = @"
                        SELECT * FROM report 
                        JOIN driver ON report.driver_id = driver.driver_id 
                        WHERE driver.driver_id = @driverId";

            return dbConnection.Query<Report>(sql,new {driverId = driverId}).ToList();
        }

        public Driver? SearchDriver(string searchTerm)
        {
            using IDbConnection db = _dbFactory.CreateConnection();

          
            string sql = @"
                SELECT * FROM driver 
                WHERE driver_license_plate LIKE @Term 
                   OR driver_first_name LIKE @Term 
                   OR driver_last_name LIKE @Term";

            return db.QueryFirstOrDefault<Driver>(sql, new { Term = $"%{searchTerm}%" });
        }

        private int GetOrAddPlatform(string platformName)
        {
            using IDbConnection db = _dbFactory.CreateConnection();

            
            string selectSql = "SELECT platform_id FROM platform WHERE platform_name = @Name";
            int? platformId = db.QueryFirstOrDefault<int?>(selectSql, new { Name = platformName });

           
            if (platformId.HasValue && platformId.Value > 0)
            {
                return platformId.Value;
            }

            string insertSql = @"
        INSERT INTO platform (platform_name) VALUES (@Name);
        SELECT CAST(SCOPE_IDENTITY() as int);";

            return db.ExecuteScalar<int>(insertSql, new { Name = platformName });
        }

        private void LinkDriverToPlatform(int driverId, int platformId)
        {
            using IDbConnection db = _dbFactory.CreateConnection();

       
            string sql = @"
        IF NOT EXISTS (SELECT 1 FROM driver_platform WHERE driver_id = @DriverId AND platform_id = @PlatformId)
        BEGIN
            INSERT INTO driver_platform (driver_id, platform_id) 
            VALUES (@DriverId, @PlatformId)
        END";

            db.Execute(sql, new { DriverId = driverId, PlatformId = platformId });
        }

        public void SubmitNewIncident(string licensePlate, string? firstName, string? lastName, string? platformName, string reason)
        {

            Driver? existingDriver = SearchDriver(licensePlate);
            int targetDriverId;

            if (existingDriver == null)
            {
                Driver newDriver = new Driver
                {
                    DriverLicensePlate = licensePlate,
                    DriverFirstName = firstName,
                    DriverLastName = lastName
                };
                targetDriverId = AddDriver(newDriver);
            }
            else
            {
                targetDriverId = existingDriver.DriverId;
            }

            if (!string.IsNullOrWhiteSpace(platformName))
            {
               
                int platformId = GetOrAddPlatform(platformName.Trim());

                
                LinkDriverToPlatform(targetDriverId, platformId);
            }

          
            Report newReport = new Report
            {
                DriverId = targetDriverId,
                ReportReason = reason,
                ReportDate = DateOnly.FromDateTime(DateTime.Now)
            };

            AddReport(newReport);
        }

        public PagedResult<Report> GetDriverReports(int driverId, int pageNumber, int pageSize)
        {
            using IDbConnection db = _dbFactory.CreateConnection();

          
            int offset = (pageNumber - 1) * pageSize;

            string sql = @"
                -- Query 1: Get the total number of reports for this driver
                SELECT COUNT(*) FROM report WHERE driver_id = @DriverId;

                -- Query 2: Get the specific page of data
                SELECT * FROM report 
                JOIN driver ON report.driver_id = driver.driver_id 
                WHERE driver.driver_id = @DriverId
                ORDER BY report_date DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

      
            using var multi = db.QueryMultiple(sql, new { DriverId = driverId, Offset = offset, PageSize = pageSize });

            int totalCount = multi.ReadFirst<int>();
            List<Report> reports = multi.Read<Report>().ToList();

            return new PagedResult<Report>
            {
                Items = reports,
                TotalCount = totalCount,
                PageSize = pageSize
            };
        }

    }
}
