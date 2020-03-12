using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        //private readonly static Dictionary<string, string> _jobs = new Dictionary<string, string>();
        private readonly static IDatabase _db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
        private readonly ILogger<JobService> _logger;

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();
            var resp = new RegisterResponse
            {
                Id = id
            };

            //_jobs[id] = request.Description;
            _db.StringSet(id, request.Description);

            return Task.FromResult(resp);
        }
    }
}