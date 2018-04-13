using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace CsvStrem
{
    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public DateTime Birthday { get; set; }

        public string ToCSVString()
        {
            return $"{ID},{Name},{TransferSex2Display(Sex)},{Birthday.ToString("yyyy/MM/dd")}";
        }

        public string ToOutputString()
        {
            return $"|{ID}|{Name}|{TransferSex2Display(Sex)}|{Birthday.ToString("yyyy/MM/dd")}|";
        }

        public string TransferSex2Display(Sex sex)
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

    public static class Extension
    {
        public static string GetEnumDisplayName(this Enum enumValue)
        {
            return enumValue.GetType().GetMember(enumValue.ToString())
                              .First()
                              .GetCustomAttribute<DisplayAttribute>()
                              .Name;

        }

        public static Dictionary<string, string> SexDic = new Dictionary<string, string>()
        {
            {Sex.Female.GetEnumDisplayName(),  Sex.Female.ToString() } ,
            {Sex.Male.GetEnumDisplayName(),  Sex.Male.ToString() }
        }; 
    }

    public enum Sex
    {
        [Display(Name = "Female")]
        Female = 0,
        [Display(Name = "Male")]
        Male = 1
    }

}
