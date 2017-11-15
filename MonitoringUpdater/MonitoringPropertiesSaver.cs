using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MonitoringUpdater
{
    internal class MonitoringPropertiesDbSaver
    {
        private readonly string _connectionString;

        public MonitoringPropertiesDbSaver(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveMonitoringDevices(IEnumerable<MonitoringDevice> monitoringDevices)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                foreach (var monitoringDevice in monitoringDevices)
                {
                    var devices = GetDeviceIdsByName(monitoringDevice.DeviceName, connection);
                    foreach (var deviceId in devices)
                    {
                        var transaction = connection.BeginTransaction();
                        SaveProperties(deviceId, monitoringDevice, connection, transaction);
                        transaction.Commit();
                    }
                }
            }
        }

        private static void DeleteProperty(int idDevice, string propertyName, SqlConnection connection, SqlTransaction transaction)
        {
            var query = @"DELETE FROM MonitoringProperties
                    WHERE [ID Device] = @IdDevice AND [Property Name] = @PropertyName";
            if (propertyName == null)
            {
                query = @"DELETE FROM MonitoringProperties
                    WHERE [ID Device] = @IdDevice AND [Property Name] IS NULL";
            }
            var command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@IdDevice", idDevice);
            command.Parameters.AddWithValue("@PropertyName", (object)propertyName ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        private static void SaveProperties(int idDevice, MonitoringDevice device, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"INSERT INTO MonitoringProperties ([ID Device], [Property Name], [Property Value], [Update Date])
                    VALUES (@IdDevice, @PropertyName, @PropertyValue, @UpdateDate);";
            foreach (var property in device.Properties)
            {
                DeleteProperty(idDevice, property.Name, connection, transaction);
                var command = new SqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@IdDevice", idDevice);
                command.Parameters.AddWithValue("@PropertyName", (object)property.Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@PropertyValue", (object)property.Value ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdateDate", device.UpdateDate);
                command.ExecuteNonQuery();
            }
        }

        private IEnumerable<int> GetDeviceIdsByName(string deviceName, SqlConnection connection)
        {
            var devices = new List<int>();
            const string query = @"SELECT [d].[ID Device]
                    FROM Devices d 
                    WHERE d.[Device Name] LIKE @DeviceName";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DeviceName", deviceName + " %");
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    devices.Add(reader.GetInt32(0));
                }
            }
            reader.Close();
            return devices;
        }
    }
}
