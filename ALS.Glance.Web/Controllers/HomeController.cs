using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using ALS.Glance.Core.Cache;
using ALS.Glance.Core.Security;
using ALS.Glance.DataAgents;
using ALS.Glance.DataAgents.ALS.Glance.Api.Models;
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
        private readonly Settings _settings;
        private readonly string _apiUrl;

        public HomeController(IALSGlanceDA glanceDa, WebApiCredentials credentials, Settings settings)
        {
            _glanceDa = glanceDa;
            _credentials = credentials;
            _settings = settings;
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


        [MvcAuthorize(Roles.Admin, Roles.User)]
        public async Task<JavaScriptResult> ApiAuth(CancellationToken ct)
        {
            var auth = await WebApiODataContainer.Using(_apiUrl, _credentials)
                .AuthenticateAsync(ct);
            var script = string.Format(@"var alsglance = alsglance || {{}}; " +
                                       "alsglance.authToken = '{0}'; " +
                                       "alsglance.baseUri = '{1}';" +
                                       "alsglance.applicationId='{2}';" +
                                       "alsglance.dashboardUserId='{3}';" +
                                       "alsglance.userId='{4}';", auth.Authorization.AccessToken,
                                       _apiUrl,
                                       Settings.Default.ApplicationId,
                                       User.Identity.GetUserId(),
                _credentials.UserName);
            return JavaScript(script);
        }

        [MvcAuthorize(Roles.Admin, Roles.User)]
        public async Task<ActionResult> Patients(int? id, CancellationToken ct)
        {
            if (id == null)
            {
                return await ViewPatients(ct);
            }
            var cache = new ResponseCache<PatientViewModel>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled,
                _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
            var patientModel = cache.GetValue(Request);
            if (patientModel == null)
            {
                var muscles = await _glanceDa.GetMusclesAsync(_credentials, ct);
                var yearBounds = await _glanceDa.GetYearBoundsAsync(_credentials, id.Value, ct);
                var patient = await _glanceDa.GetPatientAsync(_credentials, id.Value, ct);
                patientModel = new PatientViewModel
               {
                   Name = patient.Name,
                   Id = id.Value,
                   YearMax = yearBounds.Max,
                   YearMin = yearBounds.Min,
                   Muscles = muscles.ToArray(),
               };
                cache.SetValue(Request, patientModel);
            }
            var settings = await _glanceDa.GetSettingsAsync(_credentials, User.Identity.GetUserId(), ct);
            patientModel.Settings = settings != null ? settings.Value : "{}";
            return View("Patient", patientModel);
        }
        #region Private Methods
        private async Task<ActionResult> ViewPatients(CancellationToken ct)
        {
            var cache = new ResponseCache<AgeBounds>(false, DefaultCacheTime.Long, _settings.ResponseCacheEnabled,
                _settings.ResponseCacheDefaultShortTimeInMinutes, _settings.ResponseCacheDefaultLongTimeInMinutes);
            var ageBounds = cache.GetValue(Request);
            if (ageBounds == null)
            {
                ageBounds = await _glanceDa.GetAgeBoundsAsync(_credentials, ct);
                cache.SetValue(Request, ageBounds);
            }

            var model = new PatientsViewModel
            {
                AgeMax = ageBounds.Max,
                AgeMin = ageBounds.Min,
            };
            return View(model);
        }
        #endregion
    }
}