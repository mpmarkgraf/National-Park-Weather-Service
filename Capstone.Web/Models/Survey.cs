using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    /// <summary>
    /// Holds properties that model the columns in the survey_result table
    /// in the NPWS database
    /// </summary>
    public class Survey
    {
        #region Properties

        public int SurveyId { get; set; }
        public string ParkCode { get; set; }
        public string EmailAddress { get; set; }
        public string State { get; set; }
        public string ActivityLevel { get; set; }

        #endregion
    }
}