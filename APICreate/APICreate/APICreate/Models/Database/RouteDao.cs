using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace APICreate.Models.Database
{
    /// <summary>
    ///  data access object
    /// </summary>
    public class RouteDao
    {
        private const string TableName = "lai_apicreate";

        public string ConnectionString { get; private set; }

        public RouteDao() : this("DevDatabase")
        {

        }

        public RouteDao(string connectionKey)
        {
            var setting = ConfigurationManager.ConnectionStrings[connectionKey];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new Exception("Error: DevDatabase connection string not found");
            }

            ConnectionString = setting.ConnectionString;
        }

        /// <summary>
        /// Insert new line to table
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public bool InsertNewLine(Route route)
        {
            int rowCnt = 0;
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" INSERT INTO {TableName} (line_name, date, description) VALUES ");
            cmdBuilder.Append(" (@line_name, @date, @description) ");

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = cmdBuilder.ToString();
                        cmd.Parameters.AddWithValue("line_name", route.LineName);
                        cmd.Parameters.AddWithValue("date", route.Date);
                        cmd.Parameters.AddWithValue("description", route.Description);

                        rowCnt = cmd.ExecuteNonQuery();
                    }
                }
                    }
            catch(Exception ex)
            {
                throw ex;
            }

            return rowCnt > 0 ? true : false;
        }

        /// <summary>
        /// Find the route data according the critera
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IEnumerable<Route> FindRoute(Criteria criteria)
        {
            List<string> wheres = new List<string>();
            if(criteria.FromDateTime.HasValue && criteria.ToDateTime.HasValue)
            {
                wheres.Add($" date BETWEEN '{criteria.FromDateTime.Value.ToString("yyyy-MM-dd")}' AND '{criteria.ToDateTime.Value.ToString("yyyy-MM-dd")}' ");
            }

            // TODO: using full text search might improve the performance
            if(!string.IsNullOrWhiteSpace(criteria.LineName))
            {
                wheres.Add($" line_name like '%{criteria.LineName}%' ");
            }

            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" SELECT * FROM {TableName} ");
           
            if(wheres.Any())
            {
                cmdBuilder.Append($" WHERE ");
                cmdBuilder.Append(wheres.Aggregate((a, b) => $"{a} AND {b}"));
            }

            if (!string.IsNullOrWhiteSpace(criteria.OrderBy))
            {
                cmdBuilder.Append($" ORDER BY date {criteria.OrderBy} ");
            }

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int idx = 0;
                            yield return new Route()
                            {
                                Id = reader.GetInt32(idx++),
                                LineName = reader.GetString(idx++),
                                Date = reader.GetDateTime(idx++),
                                Description = reader.GetString(idx++)
                            };
                        }
                    }
                }
            }

         
        }

    }
}