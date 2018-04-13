using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public static class Extension
    {
        public static string GetEnumDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                              .First()
                              .GetCustomAttribute<DisplayAttribute>()
                              .Name;

        }

        // can not changed
        public readonly static Dictionary<string, string> SexDic = new Dictionary<string, string>()
        {
            {Sex.Female.GetEnumDisplayName(),  Sex.Female.ToString() } ,
            {Sex.Male.GetEnumDisplayName(),  Sex.Male.ToString() }
        };

        public static string TransferSex2Display(Sex sex)
        {
            var displayName = string.Empty;
            switch (sex)
            {
                case Sex.Female:
                    displayName = Sex.Female.GetEnumDisplayName();
                    break;
                case Sex.Male:
                    displayName = Sex.Male.GetEnumDisplayName();
                    break;
            }

            return displayName;
        }
    }

    public partial class Person
    {
        public string ToOutputString()
        {
            return $"|{Id}|{Name}|{Sex}|{Birthday.ToString("yyyy/MM/dd")}|";
        }
    }

    public enum DBActions
    {
        None = 0,
        Insert,
        Update,
    }

    public enum Sex
    {
        [Display(Name = "Female")]
        Female = 0,
        [Display(Name = "Male")]
        Male = 1
    }
}
