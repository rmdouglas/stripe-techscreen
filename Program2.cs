using System;
using System.Collections.Generic;
using System.Linq;

/*
  Throttle user requests
  - Every user has the same throttling permissions
  - Rolling window of requests
  - 
*/
namespace Solution
{
    using UserId = Guid;
    public interface HasTimeNow
    {
        DateTime GetTimeNow();
    }

    public class WebRequest<T>
    {
        public T Call<E, T>(E env, UserId userId)
        {
            // Throttling
            return default(T);
        }
    }
    public class Throttle
    {

        public class RequestThrottleException : Exception { }
        public struct Request
        {
            public UserId UserId;
            public DateTime TimeStamp;

            public Request(UserId user, DateTime timestamp)
            {
                this.UserId = user;
                this.TimeStamp = timestamp;
            }
        }
        private static int MaxRequests = 5;
        private static Dictionary<UserId, LinkedList<Request>> requests = new Dictionary<UserId, LinkedList<Request>>();
        public static T Call<E, T>(E env, UserId user, WebRequest<T> request)
            where E : HasTimeNow
        {
            var now = env.GetTimeNow();
            Console.WriteLine($"Throttle request at {now}");
            // See if the user has exhausted their requests for the current window
            if (requests.ContainsKey(user))
            {
                Console.WriteLine($"Checking for expired requests...");
                var expiry = now.AddMinutes(-1);
                while (requests[user].Count > 0 && requests[user].First.Value.TimeStamp <= expiry)
                {
                    Console.WriteLine($"Removing expired request for {user}");

                    requests[user].RemoveFirst();
                }
                Console.WriteLine($"{requests[user].Count()} unexpired requests");
                if (requests[user].Count() >= MaxRequests)
                {
                    // If so, throw exception
                    throw new RequestThrottleException();
                }
            }
            // If not, run the request and update the # requests
            if (!requests.ContainsKey(user))
            {
                Console.WriteLine($"Adding requests list for {user}");
                requests.Add(user, new LinkedList<Request>());
            }

            requests[user].AddLast(new Request(user, now));
            Console.WriteLine($"Adding request to list for {user}, {requests[user].Count()}");

            return request.Call<E, T>(env, user);
        }
    }
    public class Program
    {
        public class UtcTimeNow : HasTimeNow
        {
            public DateTime GetTimeNow()
            {
                return DateTime.UtcNow;
            }
        }
        public class MockGetTimeNow : HasTimeNow
        {
            public DateTime Now { get; set; }

            public DateTime GetTimeNow()
            {
                return Now;
            }
        }
        public static void Main(string[] args)
        {
            var mockTime = new MockGetTimeNow();
            mockTime.Now = DateTime.UtcNow;
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            // Test Throttling for 1 user
            try
            {
                for (int i = 0; i < 6; ++i)
                {
                    Console.WriteLine($"Making request {i} for {user1}");
                    Throttle.Call(mockTime, user1, new WebRequest<int>());
                }
            }
            catch (Throttle.RequestThrottleException ex)
            {
                Console.WriteLine("Caught exception - hooray!");
            }
            // Test Throttling for user2
            try
            {
                for (int i = 0; i < 6; ++i)
                {
                    Console.WriteLine($"Making request {i} for {user2}");
                    Throttle.Call(mockTime, user2, new WebRequest<int>());
                    try
                    {
                        Console.WriteLine($"Making additional request for {user1}");
                        Throttle.Call(mockTime, user1, new WebRequest<int>());
                    }
                    catch (Throttle.RequestThrottleException ex)
                    {
                        Console.WriteLine("Caught exception - hooray!");
                    }
                }
            }
            catch (Throttle.RequestThrottleException ex)
            {
                Console.WriteLine("Caught exception - hooray!");
            }

            mockTime.Now = mockTime.Now.AddMinutes(1);
            Console.WriteLine($"Making additional for {user1}");
            Throttle.Call(mockTime, user1, new WebRequest<int>());
        }
    }
}