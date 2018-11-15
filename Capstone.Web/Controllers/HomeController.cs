using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Capstone.Web.DAL;
using Capstone.Web.Models;

namespace Capstone.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Member Variables

        /// <summary>
        /// Creates an IParksDAL object and sets it to null, later used by the HomeController Constructor
        /// </summary>
        private IParksDAL _dal = null;
        /// <summary>
        /// String used for Session[Key]
        /// </summary>
        private const string _tempConvertKey = "Temp_Convert";

        #endregion

        #region Controller Actions

        /// <summary>
        /// Contructs HomeController, passes in an IParksDAL object
        /// </summary>
        /// <param name="dal"></param>
        public HomeController(IParksDAL dal)
        {
            _dal = dal;
        }

        /// <summary>
        /// Gets a list of all Parks in the NPWS database and passes it to the Index View
        /// </summary>
        /// <returns>Index View</returns>
        // GET: Home
        public ActionResult Index()
        {
            List<Park> parks = _dal.GetAllParks();

            return View("Index", parks);
        }

        /// <summary>
        /// Passes in string for Park Code, creates a DetailViewModel object, 
        /// sets the Park property, sets the FiveDayForecast property, checks
        /// if SessionData is null, assigns a char value if not, then passes
        /// the DetailViewModel object to the Detail View and returns the View
        /// </summary>
        /// <param name="id">Park Code</param>
        /// <returns>Detail View, DetailViewModel object</returns>
        // GET: Detail
        public ActionResult Detail(string id)
        {
            DetailViewModel detail = new DetailViewModel();
            detail.Park = _dal.GetPark(id);
            detail.FivedayForecast = _dal.GetFiveDayForecast(id);

            if (Session[_tempConvertKey] != null)
            {
                if ((char)Session[_tempConvertKey] == 'C')
                {
                    detail.isCelcius = true;
                }
                else if ((char)Session[_tempConvertKey] == 'F')
                {
                    detail.isCelcius = false;
                }
            }

            return View("Detail", detail);
        }

        /// <summary>
        /// Passes in temperature format char, sets it to SessionData,
        /// and redirects to Detail View
        /// </summary>
        /// <param name="temp">temperature format (C or F)</param>
        /// <returns>Redirects to Detail View</returns>
        //POST: Detail
        [HttpPost]
        public ActionResult Detail(char temp)
        {
            Session[_tempConvertKey] = temp;

            return RedirectToAction("Detail");
        }

        /// <summary>
        /// Creates a list of all Parks in the NPWS database and
        /// returns the list and the Survey View
        /// </summary>
        /// <returns>Surevey View, list of all Parks in DB</returns>
        // GET: Survey
        [HttpGet]
        public ActionResult Survey()
        {
            List<Park> parks = _dal.GetAllParks();

            return View("Survey", parks);
        }

        /// <summary>
        /// Validates form data from the Survey View, if valid redirects to 
        /// SurveyResult View, if not returns Survey View and a list of all Parks
        /// </summary>
        /// <param name="model">Survey form data</param>
        /// <returns>Redirects to SurveyResult if valid, or returns Survey View and list of Parks if not</returns>
        // POST: Survey
        [HttpPost]
        public ActionResult SurveySubmit(SurveyViewModel model)
        {
            if (model.State != null)
            {
                _dal.SubmitSurvey(model);
                return RedirectToAction("SurveyResult");
            }
            List<Park> parks = _dal.GetAllParks();

            return View("Survey", parks);
        }

        /// <summary>
        /// Creates a dictionary of Parks with the most submitted surveys, 
        /// sorted descending, returns SurveyResult View and dictionary
        /// </summary>
        /// <returns>SurveyResult View and dictionary of top Parks</returns>
        // GET: SurveyResult
        [HttpGet]
        public ActionResult SurveyResult()
        {
            Dictionary<string, int> surveys = _dal.GetTopParks();

            return View("SurveyResult", surveys);
        }
        #endregion
    }
}