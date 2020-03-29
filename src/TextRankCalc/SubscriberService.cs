using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATS.Client;
using NATS.Client.Rx;
using NATS.Client.Rx.Ops;
using StackExchange.Redis;

namespace TextRankCalc
{
    class SubscriberService
    {
        private readonly static IDatabase _db = ConnectionMultiplexer.Connect("localhost").GetDatabase();
        
        private readonly static ISet<char> _vowelLetters = new SortedSet<char>
        {
            'a',
            'A',
            'e',
            'E',
            'i',
            'I',
            'o',
            'O',
            'u',
            'U',
            'y',
            'Y'
        };

        private readonly static ISet<char> _consonantLetters = new SortedSet<char>
        {
            'b',
            'B',
            'c',
            'C',
            'd',
            'D',
            'f',
            'F',
            'g',
            'G',
            'h',
            'H',
            'j',
            'J',
            'k',
            'K',
            'l',
            'L',
            'm',
            'M',
            'n',
            'N',
            'p',
            'P',
            'q',
            'Q',
            'r',
            'R',
            's',
            'S',
            't',
            'T',
            'v',
            'V',
            'w',
            'W',
            'x',
            'X',
            'z',
            'Z'
        };

        public void Run(IConnection connection)
        {
            var greetings = connection.Observe("JobCreated")
                    .Where(m => m.Data?.Any() == true)
                    .Select(m => Encoding.Default.GetString(m.Data));

            greetings.Subscribe(msg =>
            {
                string data = _db.HashGet(msg, "data");
                int vowelLettersCount = 0;
                int consonantLettersCount = 0;
                foreach (char ch in data)
                {
                    if (_vowelLetters.Contains(ch))
                    {
                        ++vowelLettersCount;
                    }
                    else if (_consonantLetters.Contains(ch))
                    {
                        ++consonantLettersCount;
                    }
                }

                _db.HashSet(msg, "text_rank", $"{vowelLettersCount}/{consonantLettersCount}");
            });
        }
    }
}
