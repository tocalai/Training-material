using APICreate.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APICreate.DataAnnotations
{
    /// <summary>
    /// Validate the  date time range from input parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class DateTimeRangeValidateAttribute : ValidationAttribute
    {
        /// <summary>
        ///  customize the validation attribute of date time range
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateTime = value.ToString();
            if(string.IsNullOrWhiteSpace(dateTime))
            {
                return ValidationResult.Success;
            }

            if(!ParserHandler.Instance.ValidateDateTimeRange(dateTime))
            {
                ErrorMessage = $"{dateTime} format were unexpected, the format must follow: {{yyyy/MM/dd}}-{{yyyy/MM/dd}}";
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
       
    }
}