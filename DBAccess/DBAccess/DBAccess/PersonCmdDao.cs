using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Npgsql;

namespace DBAccess
{
    class PersonCmdDao : IPersonDao
    {

        public string ConnectionString { get; set; }

        public const string TableName = "lai_data";

        public PersonCmdDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public DBActions InsertOrUpdate(Person newPerson)
        {
            var act = DBActions.None;

            if (FindPerson(newPerson.Id) == null)
            {
                act = DBActions.Insert;
                Insert(newPerson);
            }
            else
            {
                act = DBActions.Update;
                Update(newPerson);
            }

            return act;

        }

        private void Insert(Person newPerson)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" INSERT INTO {TableName} (name, sex, birthday) VALUES ");
            cmdBuilder.Append(" (@name, @sex, @birthday) ");
            cmdBuilder.Append(" RETURNING id "); // get the insert id

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("name", newPerson.Name);
                    cmd.Parameters.AddWithValue("sex", newPerson.Sex);
                    cmd.Parameters.AddWithValue("birthday", newPerson.Birthday);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newPerson.Id = reader.GetInt32(0);
                        }
                    }
                }
            }
        }

        private void Update(Person thePerson)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" UPDATE {TableName} ");
            cmdBuilder.Append(" SET name = @name, ");
            cmdBuilder.Append(" sex = @sex,  ");
            cmdBuilder.Append(" birthday = @birthday, ");
            cmdBuilder.Append(" utime = @updateTime ");
            cmdBuilder.Append(" WHERE id = @id ");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("name", thePerson.Name);
                    cmd.Parameters.AddWithValue("sex", thePerson.Sex);
                    cmd.Parameters.AddWithValue("birthday", thePerson.Birthday);
                    cmd.Parameters.AddWithValue("updateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("id", thePerson.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DBActions InsertOrUpdatOnConflict(Person newPerson)
        {
            var act = DBActions.None;
            if (FindPerson(newPerson.Id) == null)
            {
                act = DBActions.Insert;
            }
            else
            {
                act = DBActions.Update;
            }

            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" INSERT INTO {TableName} (id, name, sex, birthday) VALUES ");
            cmdBuilder.Append(" (@id, @name, @sex, @birthday) ");
            cmdBuilder.Append(" ON CONFLICT (id) DO UPDATE SET ");
            cmdBuilder.Append(" name = @updateName, ");
            cmdBuilder.Append(" sex = @updateSex, ");
            cmdBuilder.Append(" birthday = @updateBirthday, ");
            cmdBuilder.Append(" utime = @updateTime; ");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("id", newPerson.Id);
                    cmd.Parameters.AddWithValue("name", newPerson.Name);
                    cmd.Parameters.AddWithValue("sex", newPerson.Sex);
                    cmd.Parameters.AddWithValue("birthday", newPerson.Birthday);

                    cmd.Parameters.AddWithValue("updateName", newPerson.Name);
                    cmd.Parameters.AddWithValue("updateSex", newPerson.Sex);
                    cmd.Parameters.AddWithValue("updateBirthday", newPerson.Birthday);
                    cmd.Parameters.AddWithValue("updateTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }

            return act;

        }

        public IList<Person> GetPersonByBirthDateTimeRange(DateTime dateTime, int intervalDay)
        {
            IList<Person> queryList = new List<Person>();

            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" SELECT * FROM {TableName} ");
            cmdBuilder.Append(" WHERE to_date(EXTRACT(\"year\" from to_date(@dateTime, 'YYYY-MM-DD')) || '-' || EXTRACT(\"month\" from birthday) || '-' || EXTRACT(\"day\" from birthday), 'YYYY-MM-DD') >= ( to_date(@dateTime, 'YYYY-MM-DD') - @intervalDay) AND ");
            cmdBuilder.Append(" to_date(EXTRACT(\"year\" from  to_date(@dateTime, 'YYYY-MM-DD')) || '-' || EXTRACT(\"month\" from birthday) || '-' || EXTRACT(\"day\" from birthday), 'YYYY-MM-DD') <= ( to_date(@dateTime, 'YYYY-MM-DD') + @intervalDay) ; ");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("dateTime", dateTime.ToString("yyyy/MM/dd"));
                    cmd.Parameters.AddWithValue("intervalDay", intervalDay);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            queryList.Add(new Person()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Sex = reader.GetString(2),
                                Birthday = reader.GetDateTime(3),
                                Intime = reader.GetDateTime(4),
                                Utime = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }

            return queryList;
        }

       private Person FindPerson(int id)
        {
            StringBuilder cmdBuilder = new StringBuilder();
            cmdBuilder.Append($" SELECT * FROM {TableName} ");
            cmdBuilder.Append(" WHERE id = @id ");

            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = cmdBuilder.ToString();
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Person()
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Sex = reader.GetString(2),
                                Birthday = reader.GetDateTime(3),
                                Intime = reader.GetDateTime(4),
                                Utime = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
