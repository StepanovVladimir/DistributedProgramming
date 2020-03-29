using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BackendApi;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BackendClient.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BackendClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "/config")
                .AddJsonFile("apiHosting.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterJob([Bind("Description,Data")] RegisterRequest request)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:" + _configuration["port"]);
            var client = new Job.JobClient(channel);
            var reply = await client.RegisterAsync(request);

            return RedirectToAction("JobId", reply);
        }

        public IActionResult JobId(RegisterResponse reply)
        {
            return View(reply);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
