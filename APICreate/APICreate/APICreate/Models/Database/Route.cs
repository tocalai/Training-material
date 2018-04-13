using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APICreate.Models.Database
{
    /// <summary>
    /// Route VO mapping to table: lai_apicreate
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Identity fo route
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }
        /// <summary>
        /// Name of line
        /// </summary>
        [Required]
        [StringLength(64)]
        public string LineName { get; set; }
        /// <summary>
        /// Event date
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
        /// <summary>
        /// description
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
}