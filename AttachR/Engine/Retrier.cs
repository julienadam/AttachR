using System;
using System.Collections.Generic;
using System.Threading;

namespace AttachR.Engine
{
    public static class Retrier
    {
        public static T RunWithRetryOnException<T>(Func<T> function, int numberOfRetries = 3, TimeSpan? waitPeriod = null)
        {
            var exceptions = new List<Exception>();
            for (var i = 0; i < numberOfRetries; i++)
            {
                try
                {
                    return function();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(waitPeriod ?? TimeSpan.FromSeconds(3));
                }
            }

            throw new AggregateException(exceptions);
        }
        
        public static void RunWithRetryOnException(Action function, int numberOfRetries = 3, TimeSpan? waitPeriod = null)
        {
            var exceptions = new List<Exception>();
            for (var i = 0; i < numberOfRetries; i++)
            {
                try
                {
                    function();
                    return;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(waitPeriod ?? TimeSpan.FromSeconds(3));
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}