using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LinqToDB;

/*
 Class for connection to db, handle/access person table via LINQ
 
  @History:   
     
 */

namespace DBAccess
{
    class PersonLINQDao  : LinqToDB.Data.DataConnection, IPersonDao
    {
        public PersonLINQDao() : base("KadaiDevDB") { }

        public ITable<Person> Persons { get { return this.GetTable<Person>(); } }

        #region LINQ to access 
        public static List<Person> GetAll()
        {
            using (var dao = new PersonLINQDao())
            {
                var query = from p in dao.Persons
                            select p;
                return query.ToList();
            }

        }

        public  DBActions InsertOrUpdate(Person newPerson)
        {
            DBActions action = DBActions.None;
            using (var dao = new PersonLINQDao())
            {
                var thePerson = dao.Persons.Find(newPerson.Id);
                action = thePerson != null ? DBActions.Update : DBActions.Insert;
                
                switch(action)
                {
                    case DBActions.Insert:
                        //newPerson.Id = 0;
                        newPerson.Intime = DateTime.Now;
                        newPerson.Utime = DateTime.Now;
                        newPerson.Id = Convert.ToInt32(dao.InsertWithIdentity(newPerson));
                        break;
                    case DBActions.Update:
                        newPerson.Intime = thePerson.Intime;
                        newPerson.Utime = DateTime.Now;
                        dao.Update(newPerson);
                        break;
                }

            }

            return action;
        }

        public  IList<Person> GetPersonByBirthDateTimeRange(DateTime dateTime, int intervalDay)
        {
            using (var dao = new PersonLINQDao())
            {
                var query = from p in dao.Persons
                            where p.Birthday.Month == dateTime.Month &&
                            p.Birthday.Day >= dateTime.Date.AddDays(-1 * intervalDay).Day &&
                            p.Birthday.Day <= dateTime.Date.AddDays(intervalDay).Day
                            select p;
                return query.ToList();
            }
        }
        #endregion       
    }
}
