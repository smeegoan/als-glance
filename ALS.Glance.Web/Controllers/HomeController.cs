using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ALS.Glance.DataAgents.Interfaces;
using ALS.Glance.Models.Core;
using ALS.Glance.Web.Properties;

namespace ALS.Glance.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IALSGlanceDA _glanceDa;
        private readonly WebApiCredentials _credentials;
        private readonly string _apiUrl;

        public HomeController(IALSGlanceDA glanceDa, WebApiCredentials credentials,Settings settings)
        {
            _glanceDa = glanceDa;
            _credentials = credentials;
            _apiUrl = settings.ApiUrl;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApiAuth()
        {
            var token = _credentials.ApplicationToken;
            var script = @"var my = my || {}; my.authToken = '" + token + "'; my.baseUri = '" + _apiUrl + "';";
            return JavaScript(script);
        }

        public async Task<ActionResult> Patients(int? id)
        {
            if (id == null)
            {
                var patients = await _glanceDa.GetPatientsAsync(_credentials, CancellationToken.None);

                return View(patients);
            }

            return View("Patient", id);
        }
    }
}