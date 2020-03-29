using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using StackExchange.Redis;

namespace Logger
{
    class SubscriberService
    {
        private readonly static IDatabase _db = ConnectionMultiplexer.Connect("localhost").GetDatabase();

        public void Run(IConnection connection)
        {
            var greetings = connection.Observe("JobCreated")
                    .Where(m => m.Data?.Any() == true)
                    .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg =>
            {
                string description = _db.HashGet(msg, "description");
                Console.WriteLine($"id: {msg}; description: {description}");
            });
        }
    }
}
