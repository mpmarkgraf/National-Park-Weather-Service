using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    /// <summary>
    /// Models the data required for the Detail View,
    /// Park object displays all Park data, List of 
    /// Weather objects displays the 5- day forecast, 
    /// and IsCelsius sets a bool that later gets used 
    /// by SessionData to display the temp properties of 
    /// Weather objects as Fahrenheit or Celsius
    /// </summary>
    public class DetailViewModel
    {
        #region Properties

        public Park Park { get; set; }
        public List<Weather> FivedayForecast { get; set; }
        public bool isCelcius { get; set; } = false;

        #endregion
    }
}