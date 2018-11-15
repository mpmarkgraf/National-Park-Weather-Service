using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common.WebHost;
using Capstone.Web.DAL;
using System.Linq;

namespace Capstone.Web
{
    public class MvcApplication : NinjectHttpApplication
    {
        /// <summary>
        /// Runs when application starts
        /// </summary>
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Creates a kernel and binds IParksDAL to ParksDAL,
        /// passing in the database connection string
        /// </summary>
        /// <returns></returns>
        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            string connectionString = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
            kernel.Bind<IParksDAL>().To<ParksDAL>().WithConstructorArgument("connectionString", connectionString);

            return kernel;
        }
    }


}
