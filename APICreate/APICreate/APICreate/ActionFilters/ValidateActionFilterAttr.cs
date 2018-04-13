using APICreate.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace APICreate.ActionFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidateActionFilterAttr : ActionFilterAttribute
    {
        /// <summary>
        /// before action invoke, check the model is vaild or not
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var descriptor = actionContext.ActionDescriptor as HttpActionDescriptor;

            if (descriptor != null)
            {
                var parameters = descriptor.GetParameters();

                foreach (var parameter in parameters)
                {
                    var argument = actionContext.ActionArguments[parameter.ParameterName];

                    ExecuteValidationAttributes(parameter, argument, actionContext.ModelState);
                }
            }

            if (actionContext.ModelState.IsValid)
            {
                return;
            }

            var errors = actionContext.ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage));
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, actionContext.ModelState);

            actionContext.Response.ReasonPhrase = string.Join(";", errors);
        }

        private void ExecuteValidationAttributes(HttpParameterDescriptor parameter, object argument, ModelStateDictionary modelState)
        {
            var dateTimeValidateAttr = parameter.GetCustomAttributes<DateTimeRangeValidateAttribute>().FirstOrDefault();
            if (dateTimeValidateAttr  != null)
            {
                var isValid = dateTimeValidateAttr.IsValid(argument);
                if (!isValid)
                {
                    modelState.AddModelError(parameter.ParameterName, dateTimeValidateAttr.ErrorMessage);
                }
            }

            var orderByValidateAttr = parameter.GetCustomAttributes<OrderByValidateAttribute>().FirstOrDefault();
            if (orderByValidateAttr != null)
            {
               
                var isValid = orderByValidateAttr.IsValid(argument);
                if (!isValid)
                {
                    modelState.AddModelError(parameter.ParameterName, orderByValidateAttr.ErrorMessage);
                }
            }
        }
    }
}