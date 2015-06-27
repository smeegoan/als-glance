using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ALS.Glance.DataAgents.Interfaces;
using ALS.Glance.Models.Core;
using ALS.Glance.Web.Models;
using ALS.Glance.Web.Properties;
using ALS.Glance.Web.Security;

namespace ALS.Glance.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IALSGlanceDA _glanceDa;
        private readonly WebApiCredentials _credentials;
        private readonly string _apiUrl;

        public HomeController(IALSGlanceDA glanceDa, WebApiCredentials credentials, Settings settings)
        {
            _glanceDa = glanceDa;
            _credentials = credentials;
            _apiUrl = settings.ApiUrl;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UsabilityForm()
        {
            return View();
        }


        [MvcAuthorize(Roles.AdminRole, Roles.UserRole)]
        public JavaScriptResult ApiAuth()
        {
            var token = _credentials.ApplicationToken;
            var script = string.Format(@"var alsglance = alsglance || {{}}; " +
                                       "alsglance.authToken = '{0}'; " +
                                       "alsglance.baseUri = '{1}';" +
                                       "alsglance.applicationId='{2}';" +
                                       "alsglance.dashboardUserId='{3}';" +
                                       "alsglance.userId='{4}';", token,
                                       _apiUrl,
                                       Settings.Default.ApplicationId,
                                       User.Identity.GetUserId(),
                _credentials.UserName);
            return JavaScript(script);
        }

        [MvcAuthorize(Roles.AdminRole, Roles.UserRole)]
        public async Task<ActionResult> Patients(int? id, CancellationToken ct)
        {
            if (id == null)
            {
                var ageBounds = await _glanceDa.GetAgeBoundsAsync(_credentials, ct);

                var model = new PatientsViewModel
                {
                    AgeMax = ageBounds.Max,
                    AgeMin = ageBounds.Min,
                };
                return View(model);
            }
            var settings = await _glanceDa.GetSettingsAsync(_credentials, User.Identity.GetUserId(), ct);
            var muscles = await _glanceDa.GetMusclesAsync(_credentials, ct);
            var yearBounds = await _glanceDa.GetYearBoundsAsync(_credentials, id.Value, ct);
            var patient = await _glanceDa.GetPatientAsync(_credentials, id.Value, ct);
            var patientModel = new PatientViewModel
           {
               Name = patient.Name,
               Settings = settings != null ? settings.Value : "{}",
               Id = id.Value,
               YearMax = yearBounds.Max,
               YearMin = yearBounds.Min,
               Muscles = muscles,
           };
            return View("Patient", patientModel);
        }
    }
}