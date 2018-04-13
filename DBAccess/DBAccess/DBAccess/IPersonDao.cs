using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace DBAccess
{
    interface IPersonDao
    {
        DBActions InsertOrUpdate(Person newPerson);
        IList<Person> GetPersonByBirthDateTimeRange(DateTime dateTime, int intervalDay);
    }
}
