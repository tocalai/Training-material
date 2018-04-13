using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using APICreate.Models.Database;
using APICreate.ActionFilters;
using APICreate.DataAnnotations;
using APICreate.Models;
using APICreate.Misc;
using System.Net.Http.Formatting;

namespace APICreate.Controllers
{
    /// <summary>
    /// Controller for operate Itinerary
    /// </summary>
    [RoutePrefix("api/Itinerary")]
    public class ItineraryController : ApiController
    {
        private RouteDao dao;
        public ItineraryController() : this(new RouteDao())
        {
        }

        public ItineraryController(RouteDao dao)
        {
            this.dao = dao;
        }
        /// <summary>
        ///  Add new Line to database
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        /// <remarks>
        /// Demo add new line (line_name, date, description) to store
        /// </remarks>
        [HttpPost]
        [Route("record.asp")]
        [ValidateActionFilterAttr]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description ="Unexptecd exception")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotModified)]
        [SwaggerResponse(HttpStatusCode.OK)]
        public IHttpActionResult PostNewLine(Route newRoute)
        {
            if (newRoute == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Fatal error: input parameter was null"));
            }

            try
            {
                var rows = dao.InsertNewLine(newRoute);
                if(!rows)
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotModified, "Post insert failed"));
                }
            }
            catch(Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }

            return Ok();
        }

        /// <summary>
        /// Search line according the input criteria via parameter
        /// </summary>
        /// <param name="dateRange">Date time format in From(yyyy/MM/dd)-To(yyyy/MM/dd), left blank indicate all date</param>
        /// <param name="line">Name of line, searching via partial name</param>
        /// <param name="orderBy">Order by date, ASC/DESC, default: ASC</param>
        /// <param name="type">Ouput format, JSON/XML, default: JSON</param>
        /// <returns>IHttpActionResult</returns>
        /// <remarks>
        /// Using post to retrieve route event data
        /// </remarks>
        [HttpPost]
        [Route("getRecordAPI.asp")]
        [ValidateActionFilterAttr]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Unexptecd exception")]
        public IHttpActionResult PostSearchLine
            (
            [DateTimeRangeValidate]
            string dateRange="", 
            string line="",
            [OrderByValidate]
            string orderBy ="ASC", 
            string type="JOSN"
            )
        {           
            DateTime fromDateTime;
            DateTime toDateTime;
            var isValidate = ParserHandler.Instance.ParserDateTimeRange(dateRange, out fromDateTime, out toDateTime);

            if(!isValidate)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Error: date time range input not expected"));
            }

            var criteria = new Criteria()
            {
                FromDateTime = fromDateTime != DateTime.MinValue ? fromDateTime : (DateTime?)null,
                ToDateTime = toDateTime != DateTime.MaxValue ? toDateTime : (DateTime?)null,
                LineName = line,
                OrderBy = orderBy
            };

            string message;
            if(!criteria.CheckCriteriaValid(out message))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, message));
            }

            try
            {
                var routes = dao.FindRoute(criteria);

                var formatters = string.Compare(type.ToUpper(), "JSON") == 0 ? (MediaTypeFormatter)Configuration.Formatters.JsonFormatter :
                                           string.Compare(type.ToUpper(), "XML") == 0 ? (MediaTypeFormatter)Configuration.Formatters.XmlFormatter :
                                           (MediaTypeFormatter)Configuration.Formatters.JsonFormatter;

                // let caller handle the result if the result was empty
                return Content(HttpStatusCode.OK, routes, formatters);
                
  
            }
            catch(Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
           
        }
    }
}