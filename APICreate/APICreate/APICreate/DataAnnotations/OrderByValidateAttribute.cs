using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APICreate.DataAnnotations
{
    /// <summary>
    ///  Validate the order from input parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class OrderByValidateAttribute : ValidationAttribute
    {
        /// <summary>
        /// customize the validation attribute of order
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var order = value.ToString();
            if (string.IsNullOrWhiteSpace(order))
            {
                return ValidationResult.Success;
            }

            if(string.Compare(order, "ASC", true) != 0 && (string.Compare(order, "DESC", true) != 0))
            {
                ErrorMessage = $"{order} format were unexpected, the format must be: \"ASC\" or \"DESC\" (case insensitive) ";
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}