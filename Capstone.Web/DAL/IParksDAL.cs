using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Web.Models;

namespace Capstone.Web.DAL
{
    /// <summary>
    /// Interface enforces DAL methods for ParksDAL class
    /// </summary>
    public interface IParksDAL
    {
        #region Methods

        List<Park> GetAllParks();
        Dictionary<string, int> GetTopParks();
        Park GetPark(string parkCode);
        List<Weather> GetFiveDayForecast(string parkCode);
        void SubmitSurvey(SurveyViewModel survey);

        #endregion
    }
}
