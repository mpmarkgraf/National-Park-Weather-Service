using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Capstone.Web.Models;

namespace Capstone.Web.DAL
{
    public class ParksDAL : IParksDAL //Check to make sure tests match up with all public methods
    {
        #region Member Variables

        /// <summary>
        /// SQL database connection string bound to kernel
        /// </summary>
        private string _connectionString;

        #endregion

        #region Construsctor

        /// <summary>
        /// Constructs a ParksDAL object, sets connection string to member variable
        /// </summary>
        /// <param name="connectionString">SQL database connection string</param>
        public ParksDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all Parks form NPWS database and populates a List of Park objects
        /// </summary>
        /// <returns>List of Park objects</returns>
        public List<Park> GetAllParks()
        {
            List<Park> parks = new List<Park>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string getParksSQL = "SELECT * FROM park;";

                SqlCommand cmd = new SqlCommand(getParksSQL, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    parks.Add(GetParkFromReader(reader));
                }
            }

            return parks;
        }

        /// <summary>
        /// Gets only Parks with at least 1 survey and sorts them by most surveys,
        /// if number of surveys is a tie, Parks are sorted alphabetically
        /// </summary>
        /// <returns>Dictionary of Park and its total number of surveys</returns>
        public Dictionary<string, int> GetTopParks()
        {
            Dictionary<string, int> topParks = new Dictionary<string, int>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string getParksSQL = "SELECT COUNT(park.parkCode) as count, park.parkCode, park.parkName FROM park " +
                                     "INNER JOIN survey_result ON park.parkCode = survey_result.parkCode " +
                                     "GROUP BY park.parkCode, park.parkName " +
                                     "ORDER BY COUNT(*) Desc, park.parkName;";

                SqlCommand cmd = new SqlCommand(getParksSQL, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    topParks[reader["parkName"].ToString()] = (int)reader["count"];
                }
            }

            return topParks;
        }

        /// <summary>
        /// Gets a single Park object by its Park Code
        /// </summary>
        /// <param name="parkCode">Park Code</param>
        /// <returns>Park object</returns>
        public Park GetPark(string parkCode)
        {
            Park park = new Park();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string getParkSQL = "SELECT * FROM park " +
                                     "WHERE parkCode = @parkCode;";

                SqlCommand cmd = new SqlCommand(getParkSQL, conn);
                cmd.Parameters.AddWithValue("@parkCode", parkCode);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    park = GetParkFromReader(reader);
                }
            }

            return park;
        }

        /// <summary>
        /// Creates a Park object and populates its properties from SQL Reader
        /// </summary>
        /// <param name="reader">SqlDataReader object</param>
        /// <returns>Park object</returns>
        private Park GetParkFromReader(SqlDataReader reader)
        {
            Park park = new Park();

            park.ParkCode = reader["parkCode"].ToString();
            park.ParkName = reader["parkName"].ToString();
            park.State = reader["state"].ToString();
            park.Acreage = (int)reader["acreage"];
            park.ElevationInFeet = (int)reader["elevationInFeet"];
            park.MilesOfTrail = (float)reader["milesOfTrail"];
            park.NumberOfCampsites = (int)reader["numberOfCampsites"];
            park.Climate = reader["climate"].ToString();
            park.YearFounded = (int)reader["yearFounded"];
            park.AnnualVisitorCount = (int)reader["annualVisitorCount"];
            park.InspirationQuote = reader["inspirationalQuote"].ToString();
            park.InspirationalQuoteSource = reader["inspirationalQuoteSource"].ToString();
            park.ParkDescription = reader["parkDescription"].ToString();
            park.EntryFee = (int)reader["entryFee"];
            park.NumberOfAnimalSpecies = (int)reader["numberOfAnimalSpecies"];

            return park;
        }

        /// <summary>
        /// Gets the 5-day weather forecast from the NPWS database
        /// </summary>
        /// <param name="parkCode">Park Code</param>
        /// <returns>List of Weather objects</returns>
        public List<Weather> GetFiveDayForecast(string parkCode)
        {
            List<Weather> forecast = new List<Weather>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string getForecastSQL = "SELECT * FROM weather " +
                                        "Where parkCode = @parkCode;";

                SqlCommand cmd = new SqlCommand(getForecastSQL, conn);

                cmd.Parameters.AddWithValue("@parkCode", parkCode);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    forecast.Add(GetWeatherFromReader(reader));
                }
            }

            return forecast;
        }

        /// <summary>
        /// Creates a Weather object and populates its properties from SQL Reader
        /// </summary>
        /// <param name="reader">SqlDataReader object</param>
        /// <returns>Weather object</returns>
        private Weather GetWeatherFromReader(SqlDataReader reader)
        {
            Weather weather = new Weather();

            weather.ParkCode = reader["parkCode"].ToString();
            weather.FiveDayForcastValue = (int)reader["fiveDayForecastValue"];
            weather.FahrenheitLow = (int)reader["low"];
            weather.FahrenheitHigh = (int)reader["high"];
            weather.Forecast = reader["forecast"].ToString();

            return weather;
        }

        /// <summary>
        /// Inserts survey form data into NPWS database
        /// </summary>
        /// <param name="survey">Survey form data</param>
        public void SubmitSurvey(SurveyViewModel survey)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string submitSurveySQL = "INSERT INTO survey_result " +
                                         "(parkCode, emailAddress, state, activityLevel) " +
                                         "VALUES (@parkCode, @emailAddress, @state, @activityLevel);";

                SqlCommand cmd = new SqlCommand(submitSurveySQL, conn);
                cmd.Parameters.AddWithValue("@parkCode", survey.ParkCode);
                cmd.Parameters.AddWithValue("@emailAddress", survey.EmailAddress);
                cmd.Parameters.AddWithValue("@state", survey.State);
                cmd.Parameters.AddWithValue("@activityLevel", survey.ActivityLevel);

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}