using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIGET.WeacherViewer.Misc
{
    public class ErrorHandlerExtAttr : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (IsAjax(filterContext))
            {
                // exception raised after ajax invocation
                // return Json
                filterContext.Result = new JsonResult()
                {
                    Data = filterContext.Exception.Message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
            }
            else
            {
                // normal Exception
                // so let it handle by its default ways.
                base.OnException(filterContext);

            }



        }

        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}