using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Capstone.Web.Models
{
    /// <summary>
    /// Models the data required for the Survey View,
    /// enforces validaitoni of survey forms
    /// </summary>
    public class SurveyViewModel
    {
        #region Properties

        [Required]
        public string ParkCode { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string ActivityLevel { get; set; }

        #endregion
    }
}