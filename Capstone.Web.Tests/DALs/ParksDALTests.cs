using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Capstone.Web.DAL;
using Capstone.Web.Models;
using System.Transactions;
using System.Data.SqlClient;

namespace Capstone.Web.Tests.DALs
{
    /// <summary>
    /// Integration tests for ParksDAL
    /// </summary>
    [TestClass]
    public class ParksDALTests
    {

        #region Member Variables

        static string _connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=NPWS;Integrated Security=true";
        private int _parksCount;
        private int _surveyId;

        private List<Weather> _forecast = new List<Weather>();
        private Dictionary<string, int> _topParks = new Dictionary<string, int>();

        private TransactionScope tran = null;
        private ParksDAL _parksDal = new ParksDAL(_connectionString);
        private Park _park = new Park();
        private SurveyViewModel _survey1 = new SurveyViewModel()
        {
            ParkCode = "CVNP",
            EmailAddress = "abc@123.com",
            State = "Ohio",
            ActivityLevel = "active"
        };
        SurveyViewModel _survey2 = new SurveyViewModel()
        {
            ParkCode = "ENP",
            EmailAddress = "def@456.com",
            State = "Florida",
            ActivityLevel = "extremely active"
        };
        SurveyViewModel _survey3 = new SurveyViewModel()
        {
            ParkCode = "CVNP",
            EmailAddress = "ghi@789.com",
            State = "California",
            ActivityLevel = "active"
        };

        #endregion
        
        #region Test Initialize

        /// <summary>
        /// Initializes integration tests, gathers database
        /// info and sets member variables to later be used by tests
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            tran = new TransactionScope();
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlCommand cmd;
                
                cmd = new SqlCommand("SELECT COUNT(*) FROM park;", conn);
                _parksCount = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand("SELECT TOP 1 * FROM park;", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _park.ParkCode = reader["parkCode"].ToString();
                    _park.ParkName = reader["parkName"].ToString();
                    _park.State = reader["state"].ToString();
                    _park.Acreage = (int)reader["acreage"];
                    _park.ElevationInFeet = (int)reader["elevationInFeet"];
                    _park.MilesOfTrail = (float)reader["milesOfTrail"];
                    _park.NumberOfCampsites = (int)reader["numberOfCampsites"];
                    _park.Climate = reader["climate"].ToString();
                    _park.YearFounded = (int)reader["yearFounded"];
                    _park.AnnualVisitorCount = (int)reader["annualVisitorCount"];
                    _park.InspirationQuote = reader["inspirationalQuote"].ToString();
                    _park.InspirationalQuoteSource = reader["inspirationalQuoteSource"].ToString();
                    _park.ParkDescription = reader["parkDescription"].ToString();
                    _park.EntryFee = (int)reader["entryFee"];
                    _park.NumberOfAnimalSpecies = (int)reader["numberOfAnimalSpecies"];
                }

                reader.Close();

                cmd = new SqlCommand($"SELECT * FROM weather WHERE parkCode = '{_park.ParkCode}';", conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Weather weather = new Weather();

                    weather.ParkCode = reader["parkCode"].ToString();
                    weather.FiveDayForcastValue = (int)reader["fiveDayForecastValue"];
                    weather.FahrenheitLow = (int)reader["low"];
                    weather.FahrenheitHigh = (int)reader["high"];
                    weather.Forecast = reader["forecast"].ToString();

                    _forecast.Add(weather);
                }

                reader.Close();

                cmd = new SqlCommand($"INSERT INTO survey_result (parkCode, emailAddress, state, activityLevel) " +
                                     $"VALUES ('{_survey1.ParkCode}', '{_survey1.EmailAddress}', '{_survey1.State}', " +
                                     $"'{_survey1.ActivityLevel}'); SELECT CAST(SCOPE_IDENTITY() as int);", conn);
                _surveyId = (int)cmd.ExecuteScalar();

                cmd = new SqlCommand($"INSERT INTO survey_result (parkCode, emailAddress, state, activityLevel) " +
                                     $"VALUES ('{_survey2.ParkCode}', '{_survey2.EmailAddress}', '{_survey2.State}', " +
                                     $"'{_survey1.ActivityLevel}');", conn);
                cmd.ExecuteScalar();

                cmd = new SqlCommand($"INSERT INTO survey_result (parkCode, emailAddress, state, activityLevel) " +
                                     $"VALUES ('{_survey3.ParkCode}', '{_survey3.EmailAddress}', '{_survey3.State}', " +
                                     $"'{_survey3.ActivityLevel}');", conn);
                cmd.ExecuteScalar();
                
                cmd = new SqlCommand("SELECT COUNT(park.parkCode) as count, park.parkCode, park.parkName FROM park " +
                                     "INNER JOIN survey_result ON park.parkCode = survey_result.parkCode " +
                                     "GROUP BY park.parkCode, park.parkName " +
                                     "ORDER BY COUNT(*) Desc, park.parkName;", conn);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    _topParks[reader["parkName"].ToString()] = (int)reader["count"];
                }
            }
        }

        #endregion

        #region Test Cleanup

        [TestCleanup]
        public void Cleanup()
        {
            tran.Dispose();
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Tests GetAllParks DAL method,
        /// asserts that list count is equal
        /// to member variable list count
        /// </summary>
        [TestMethod]
        public void GetAllParksTest()
        {
            List<Park> parks = new List<Park>();

            parks = _parksDal.GetAllParks();

            Assert.AreEqual(_parksCount, parks.Count);
        }

        /// <summary>
        /// Tests GetPark DAL method,
        /// asserts Park properties are equal
        /// to member variable Park object properties
        /// </summary>
        [TestMethod]
        public void GetParkTest()
        {
            Park park = new Park();

            park = _parksDal.GetPark(_park.ParkCode);

            Assert.AreEqual(_park.ParkCode, park.ParkCode);
            Assert.AreEqual(_park.ParkName, park.ParkName);
            Assert.AreEqual(_park.State, park.State);
            Assert.AreEqual(_park.Acreage, park.Acreage);
            Assert.AreEqual(_park.ElevationInFeet, park.ElevationInFeet);
            Assert.AreEqual(_park.MilesOfTrail, park.MilesOfTrail);
            Assert.AreEqual(_park.NumberOfCampsites, park.NumberOfCampsites);
            Assert.AreEqual(_park.Climate, park.Climate);
            Assert.AreEqual(_park.YearFounded, park.YearFounded);
            Assert.AreEqual(_park.AnnualVisitorCount, park.AnnualVisitorCount);
            Assert.AreEqual(_park.InspirationQuote, park.InspirationQuote);
            Assert.AreEqual(_park.InspirationalQuoteSource, park.InspirationalQuoteSource);
            Assert.AreEqual(_park.ParkDescription, park.ParkDescription);
            Assert.AreEqual(_park.EntryFee, park.EntryFee);
            Assert.AreEqual(_park.NumberOfAnimalSpecies, park.NumberOfAnimalSpecies);
        }

        /// <summary>
        /// Tests GetFiveDayForecast DAL method,
        /// asserts that list count is equal to
        /// member variable list count, and asserts
        /// that all properties match member variable
        /// List<Weather> object properties
        /// </summary>
        [TestMethod]
        public void GetFiveDayForecastTest()
        {
            List<Weather> forecast = new List<Weather>();
            forecast = _parksDal.GetFiveDayForecast(_park.ParkCode);

            Assert.AreEqual(_forecast.Count, forecast.Count);
            Assert.AreEqual(_forecast[0].ParkCode, forecast[0].ParkCode);
            Assert.AreEqual(_forecast[1].FiveDayForcastValue, forecast[1].FiveDayForcastValue);
            Assert.AreEqual(_forecast[2].FahrenheitLow, forecast[2].FahrenheitLow);
            Assert.AreEqual(_forecast[3].FahrenheitHigh, forecast[3].FahrenheitHigh);
            Assert.AreEqual(_forecast[4].Forecast, forecast[4].Forecast);
        }
        
        /// <summary>
        /// Tests SubmitSurvey DAL method,
        /// asserts the surveyId for newly added survey
        /// matches the SQL search for the last added surveyId
        /// </summary>
        [TestMethod]
        public void SubmitSurveyTest()
        {
            int _newSurveyId = 0;
            _parksDal.SubmitSurvey(_survey1);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string getNewSurveyId = "SELECT TOP 1 surveyId FROM survey_result " +
                                        "ORDER BY surveyId Desc;";

                SqlCommand cmd = new SqlCommand(getNewSurveyId, conn);

                _newSurveyId = (int)cmd.ExecuteScalar();
            }

                Assert.AreEqual(_surveyId + 3, _newSurveyId);
        }

        /// <summary>
        /// Tests GetTopParks DAL method,
        /// asserts the dictionary count is equal
        /// to the member variable dictionary count,
        /// and iterates through dictionary checking if
        /// values are equal to member variable
        /// dictionary items
        /// </summary>
        [TestMethod]
        public void GetTopParksTest()
        {
            Dictionary<string, int> topParks = new Dictionary<string, int>();

            topParks = _parksDal.GetTopParks();

            Assert.AreEqual(_topParks.Count, topParks.Count);

            foreach (KeyValuePair<string, int> item in topParks)
            {
                Assert.AreEqual(_topParks[item.Key], item.Value);
            }
        }

        #endregion
    }
}
