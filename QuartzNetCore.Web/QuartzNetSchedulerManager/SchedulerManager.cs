using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace QuartzNetCore.Web.QuartzNetSchedulerManager
{
    public class SchedulerManager
    {
        static readonly object Locker = new object();
        static IScheduler _scheduler;
        static readonly ConcurrentDictionary<string, IScheduler> ConnectionCache = new ConcurrentDictionary<string, IScheduler>();
        const string ChannelType = "tcp";
        const string LocalIp = "127.0.0.1";
        const string Port = "555";
        const string BindName = "QuartzScheduler";
        public static IScheduler Scheduler
        {
            get
            {
                if (_scheduler == null)
                {
                    lock (Locker)
                    {
                        if (_scheduler == null)
                        {
                            _scheduler = GetScheduler(LocalIp);
                        }
                    }
                }
                return _scheduler;
            }
        }
        private static IScheduler GetScheduler(string ip)
        {
            if (!ConnectionCache.ContainsKey(ip))
            {
                var properties = new NameValueCollection
                {
                    ["quartz.scheduler.proxy"] = "true",
                    ["quartz.scheduler.proxy.address"] = $"{ChannelType}://{LocalIp}:{Port}/{BindName}"
                };
                var schedulerFactory = new StdSchedulerFactory(properties);
                _scheduler = schedulerFactory.GetScheduler().Result;
                ConnectionCache[ip] = _scheduler;
            }
            return ConnectionCache[ip];
        }
    }
}
