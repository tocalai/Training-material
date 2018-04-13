using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIGET.Weather.Core.Database
{
    public class CityDao
    {
        private const string TableName = "lai_apiget";

        public string ConnectionString { get; private set; }

        public CityDao() : this("DevDatabase")
        {

        }

        public CityDao(string connectionKey)
        {
            var setting = ConfigurationManager.ConnectionStrings[connectionKey];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new Exception("Error: DevDatabase connection string not found");
            }

            ConnectionString = setting.ConnectionString;
        }

        private IEnumerable<City> Convert2City(DbDataReader  reader)
        {
            using (reader)
            {
                while (reader.Read())
                {
                    yield return new City()
                    {
                        Id = reader.GetInt32(0),
                        CityName = reader.GetString(1),
                        CityId = reader.GetString(2),
                        Date = reader.GetDateTime(3),
                        WeatherDescription = reader.GetString(4),
                        MaxTemperatureCelsius = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                        MinTemperatureCelsius = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                        InsertTime = reader.GetDateTime(7),
                        UpdateTime = reader.GetDateTime(8)
                    };
                }
            }
            //return null;
        }

        public async Task<IEnumerable<City>> GetWeatherInfos(IEnumerable<string> wheres)
        {
            List<City> citys = new List<City>();

            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" SELECT * FROM {TableName} ");
            cmdBuilder.Append(" WHERE (city_id, date) IN ");
            cmdBuilder.Append("(" + wheres.Aggregate((a, b) => $"{a},{b}") + ")");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();

                    var reader = await cmd.ExecuteReaderAsync();
                    citys.AddRange(Convert2City(reader));
                }
            }

            return citys.Count > 0 ? citys : null;
        }

        public void InsertOnConfilct(City city)
        {
            if(FindCityWeather(city.CityId, city.Date) == null)
            {
                InsertCity(city);
            }
            else
            {
                UpdateCity(city);
            }
        }

        public void InsertCity(City newCity)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" INSERT INTO {TableName} ( city_name, city_id, date, description, max_temperature_celsius, min_temperature_celsius, intime, utime) VALUES ");
            cmdBuilder.Append(" (@city_name, @city_id, @date, @description, @max_temperature, @min_temperature, @intime, @utime) ");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("city_name", newCity.CityName);
                    cmd.Parameters.AddWithValue("city_id", newCity.CityId);
                    cmd.Parameters.AddWithValue("date", newCity.Date);
                    cmd.Parameters.AddWithValue("description", newCity.WeatherDescription);
                    cmd.Parameters.AddWithValue("max_temperature", newCity.MaxTemperatureCelsius ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("min_temperature", newCity.MinTemperatureCelsius ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("intime", DateTime.Now);
                    cmd.Parameters.AddWithValue("utime", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCity(City theCity)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" UPDATE {TableName} ");
            cmdBuilder.Append(" SET city_name  = @name, ");
            cmdBuilder.Append(" description = @description,  ");
            cmdBuilder.Append(" max_temperature_celsius = @max_temperature, ");
            cmdBuilder.Append(" min_temperature_celsius = @min_temperature, ");
            cmdBuilder.Append(" utime  = @utime ");
            cmdBuilder.Append(" WHERE city_id = @cityId  and date = @date");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("name", theCity.CityName);
                    cmd.Parameters.AddWithValue("cityId", theCity.CityId);
                    cmd.Parameters.AddWithValue("date", theCity.Date);
                    cmd.Parameters.AddWithValue("description", theCity.WeatherDescription);
                    cmd.Parameters.AddWithValue("max_temperature", theCity.MaxTemperatureCelsius ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("min_temperature", theCity.MinTemperatureCelsius ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("utime", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public City FindCityWeather(string cityId, DateTime forecastDateTime)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" SELECT * FROM {TableName} ");
            cmdBuilder.Append($" WHERE city_id = @cityId AND date = @forecastDate ");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("cityId", cityId);
                    cmd.Parameters.AddWithValue("forecastDate", forecastDateTime);

                    var ret = Convert2City(cmd.ExecuteReader());
                    return ret.FirstOrDefault();
                }
            }
        }
    }
}
