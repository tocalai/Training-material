using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace APICreate.Misc
{
    /// <summary>
    /// Helper class for handling string parsing
    /// </summary>
    public class ParserHandler
    {
        private static readonly Lazy<ParserHandler> lazy = new Lazy<ParserHandler>(() => new ParserHandler());

        public static ParserHandler Instance { get { return lazy.Value; } }
        private ParserHandler()
        {

        }

        public bool ValidateDateTimeRange(string dateTimeStr)
        {
            string pattern = "^[1-2][0-9][0-9][0-9]/[0-1]?[0-9]/[0-3]?[0-9]-[1-2][0-9][0-9][0-9]/[0-1]?[0-9]/[0-3]?[0-9]$";

            Regex regex = new Regex(pattern);
            if(!regex.IsMatch(dateTimeStr))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse date time 
        /// </summary>
        /// <param name="dateTimeStr"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <returns></returns>
        public bool ParserDateTimeRange(string dateTimeStr, out DateTime fromDateTime, out DateTime toDateTime)
        {
            fromDateTime = DateTime.MinValue;
            toDateTime = DateTime.MaxValue;

            if(string.IsNullOrEmpty(dateTimeStr))
            {
                return true;
            }

            if(!ValidateDateTimeRange(dateTimeStr))
            {
                return false;
            }

            var apart = dateTimeStr.Split('-');

            if(!DateTime.TryParse(apart[0], out fromDateTime) ||  !DateTime.TryParse(apart[1], out toDateTime))
            {
                return false;
            }

            return true;
        }

    }
}