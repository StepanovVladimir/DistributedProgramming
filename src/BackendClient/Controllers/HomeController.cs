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
        private readonly GrpcChannel _channel;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "/config")
                .AddJsonFile("apiHosting.json", optional: true, reloadOnChange: true)
                .Build();
            _channel = GrpcChannel.ForAddress("http://localhost:" + configuration["port"]);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterJob([Bind("Description,Data")] RegisterRequest request)
        {
            var client = new Job.JobClient(_channel);
            var reply = await client.RegisterAsync(request);

            return RedirectToAction("TextDetails", reply);
        }

        public IActionResult TextDetails(RegisterResponse jobId)
        {
            var client = new Job.JobClient(_channel);
            var reply = client.GetProcessingResult(jobId);

            return View(reply);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
