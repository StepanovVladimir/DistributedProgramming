using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using NATS.Client;
using StackExchange.Redis;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        private readonly static IDatabase _db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
        private readonly ILogger<JobService> _logger;

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();

            _db.StringSet(id, request.Description);

            string natsConnectionString = "nats://127.0.0.1";
            var greeterService = new GreeterService();
            using (IConnection connection = new ConnectionFactory().CreateConnection(natsConnectionString))
            {
                greeterService.Run(connection, id);
            }

            var resp = new RegisterResponse
            {
                Id = id
            };
            return Task.FromResult(resp);
        }
    }
}