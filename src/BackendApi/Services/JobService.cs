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
        private readonly static GreeterService _greeterService = new GreeterService();
        private readonly static IConnection _connection = new ConnectionFactory().CreateConnection("nats://127.0.0.1");

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();

            _db.HashSet(id, "description", request.Description);
            _db.HashSet(id, "data", request.Data);

            _greeterService.Run(_connection, id);

            var resp = new RegisterResponse
            {
                Id = id
            };

            return Task.FromResult(resp);
        }
    }
}