using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ALS.Glance.DataAgents.Interfaces;
using ALS.Glance.Models.Core;
using ALS.Glance.Web.Models;
using ALS.Glance.Web.Properties;

namespace ALS.Glance.Web.Controllers
{//roles user.api e user.site
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

        public ActionResult ApiAuth()
        {
            var token = _credentials.ApplicationToken;
            var script = @"var als_glance = als_glance || {}; als_glance.authToken = '" + token + "'; als_glance.baseUri = '" + _apiUrl + "';";
            return JavaScript(script);
        }

        public async Task<ActionResult> Patients(int? id, CancellationToken ct)
        {
            if (id == null)
            {
                var patients = await _glanceDa.GetPatientsAsync(_credentials, ct);

                return View(patients);
            }
            var muscles = await _glanceDa.GetMusclesAsync(_credentials, id.Value, ct);
            var years = new List<int>( await _glanceDa.GetFactYearsAsync(_credentials, id.Value, ct));
            years.Add(years.Max()+1);
            
            var model = new PatientViewModel
            {
                Id = id.Value,
                Muscles = muscles.Select(e => Tuple.Create(e.Item1, e.Item2, false)),
                Years = years.Select(e => Tuple.Create(e, true)),
            };
            return View("Patient", model);
        }
    }
}