using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APICreate.Models
{
    public class Criteria
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public string LineName { get; set; } 
        public string OrderBy { get; set; }

        /// <summary>
        /// Check critera logic validate
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool CheckCriteriaValid(out string message)
        {
            message = string.Empty;
            if(FromDateTime.HasValue && ToDateTime.HasValue && FromDateTime > ToDateTime)
            {
                message = $"{FromDateTime.Value.ToString("yyyy/MM/dd")}[From date time] should not > {ToDateTime.Value.ToString("yyyy/MM/dd")}[To date time]";
                return false;
            }

            return true;
        }
    }
}