using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.Models.Database_registers;
using Example.Services.Interfaces;
using Microsoft.Data.SqlClient;
using SqlUtilityLibrary;
using SqlUtilityLibrary.Interfaces;
namespace Example.Services
{
    public class RegisterService :IRegisterService
    {
        private readonly IDataService _database;

        public RegisterService(IDataService database)
        {
            _database = database;
        }

        public async Task AddLogAsync(Coil_register log)
        {
            string query =
                @"INSERT INTO Coils_registers
                    (
                        IPAddress,
                        Port,   
                        Action,
                        Date
                    )
                    VALUES
                    (
                       @IPAddress,
                       @Port,
                       @Action,
                       @Date
                        )";

            await _database.ExecuteNonQueryAsync(
                query,

                new SqlParameter("@IPAddress", log.IPAddress),
                new SqlParameter("@Port", log.Port),
                new SqlParameter("@Action", log.Action),
                new SqlParameter("@Date", log.Date)
                );
        }
    }
}
