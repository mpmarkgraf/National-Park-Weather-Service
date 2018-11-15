using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Web.Models
{
    /// <summary>
    /// Holds properties that model the columns in the survey_result table
    /// in the NPWS database, includes derived properties to display in the View
    /// </summary>
    public class Weather
    {
        #region Properties

        public string ParkCode { get; set; }
        public int FiveDayForcastValue { get; set; }
        public string Forecast { get; set; }
        public double FahrenheitLow { get; set; }
        public double FahrenheitHigh { get; set; }

        #endregion

        #region Derived Properties

        /// <summary>
        /// Caluculates the Celsius equivalent of the FahrenheitLow property
        /// </summary>
        public double CelsiusLow
        {
            get
            {
                return Math.Round((FahrenheitLow - 32) * 5 / 9);
            }
        }

        /// <summary>
        /// Calculates the Celsius equivalent of the FahrenheitHigh property
        /// </summary>
        public double CelsiusHigh
        {
            get
            {
                return Math.Round((FahrenheitHigh - 32) * 5 / 9);
            }
        }

        /// <summary>
        /// Returns the filename of the park image
        /// </summary>
        public string ImagePath
        {
            get
            {
                string imgStr = Forecast;

                if (Forecast.Contains(" "))
                {
                    string[] str = Forecast.Split();
                    imgStr = str[0] + str[1];
                }

                return imgStr;
            }
        }

        /// <summary>
        /// Returns a string based on the value of the Forecast property
        /// </summary>
        public string WeatherAdvisory
        {
            get
            {
                string advisory = "";
                
                if (Forecast == "rain")
                {
                    advisory = "Pack rain gear and waterproof shoes!";
                }
                else if (Forecast == "sunny")
                {
                    advisory = "Pack sunblock!";
                }
                else if (Forecast == "snow")
                {
                    advisory = "Pack snowshoes!";
                }
                else if (Forecast == "thunderstorms")
                {
                    advisory = "Seek shelter! Avoid hiking on exposed ridges!";
                }

                return advisory;
            }
        }

        /// <summary>
        /// Returns a string based on the calculated value of the 
        /// differences between either FahrenheitLow and FahrenheitHigh,
        /// or CelsiusLow and CelsiusHigh
        /// </summary>
        public string TempAdvisory
        {
            get
            {
                bool hotDay = FahrenheitHigh > 75 || CelsiusHigh > 23.888;
                bool coldDay = FahrenheitLow < 20 || CelsiusLow < -6.666;

                double tempDiffFahrenheit = FahrenheitHigh - FahrenheitLow;
                double tempDiffCelsius = CelsiusHigh - CelsiusLow;

                bool hotColdDay = tempDiffFahrenheit > 20 || tempDiffCelsius > 20;

                string advisory = "";

                if (hotDay)
                {
                    advisory = "HIGH TEMP! Bring an extra gallon of water!";
                }
                else if (coldDay)
                {
                    advisory = "LOW TEMP! Avoid exposure to frigid temperatures!";
                }
                if (hotColdDay)
                {
                    advisory += "Large temp range! Wear breathable layers!";
                }

                return advisory;
            }
        }

        #endregion
    }
}