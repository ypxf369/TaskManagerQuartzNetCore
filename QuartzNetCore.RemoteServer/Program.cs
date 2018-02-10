using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace QuartzNetCore.RemoteServer
{
    /// <summary>
    /// Quartz代码配置
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "RemoteServer";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "555";//端口号
            properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";//名称
            properties["quartz.scheduler.exporter.channelType"] = "tcp";//通道类型
            properties["quartz.scheduler.exporter.channelName"] = "httpQuartz";
            properties["quartz.scheduler.exporter.rejectRemoteRequests"] = "true";
            properties["quartz.jobStore.clustered"] = "true";//集群配置
            //指定quartz持久化数据库的配置
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";//存储类型
            properties["quartz.jobStore.tablePrefix"] = "Qrtz_";//表名前缀
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";//驱动类型
            properties["quartz.jobStore.dataSource"] = "myDS";//数据源名称
            properties["quartz.dataSource.myDS.connectionString"] = @"Data Source=.;Initial Catalog=QuartzManager;User ID=sa;Password=123456";//连接字符串
            properties["quartz.dataSource.myDS.provider"] = "SqlServer-20";//数据库版本
            properties["quartz.scheduler.instanceId"] = "AUTO";
            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddJobListener(new QuartzJobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();

        }
    }
}
