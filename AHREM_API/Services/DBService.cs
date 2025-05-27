using AHREM_API.Models;
using MySqlConnector;
using System.Data;
using System.Diagnostics;

namespace AHREM_API.Services
{
    public class DBService
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _connection;
        public DBService(IConfiguration config) 
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(_connectionString);
        }

        public List<Device> GetAllDevices()
        {
            if (_connection != null)
            {
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = $"SELECT * FROM devices";

                    MySqlDataReader sqlData = cmd.ExecuteReader();

                    List<Device> tempList = new List<Device>();

                    while (sqlData.Read())
                    {
                        tempList.Add(new Device
                        {
                            ID = sqlData.GetInt16("ID"),
                            IsActive = sqlData.GetBoolean("IsActive"),
                            Firmware = sqlData.GetString("Firmware"),
                            MAC = sqlData.GetString("MAC")
                        });
                    }
                    return tempList;
                }
            }
            return null;
        }

        public bool AddDevice(Device device)
        {
            if (_connection != null)
            {
                string query = "INSERT INTO devices (IsActive, Firmware, MAC) VALUES (@IsActive, @Firmware, @MAC)";

                _connection.Open();

                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@IsActive", device.IsActive);
                    cmd.Parameters.AddWithValue("@Firmware", device.Firmware);
                    cmd.Parameters.AddWithValue("@MAC", device.MAC);
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            return false;
        }

        public bool DeleteDevice(int id)
        {
            if (_connection != null)
            {
                _connection.Open();

                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;
                    cmd.CommandText = "DELETE FROM devices WHERE ID = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            return false;
        }

    }
}
