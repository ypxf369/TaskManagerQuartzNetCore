using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Quartz;
using QuartzNetCore.EFModel_SqlServer.Models;

namespace QuartzNetCore.Web.QuartzNetSchedulerManager
{
    public class JobHelper
    {
        private static readonly IScheduler _scheduler;

        static JobHelper()
        {
            _scheduler = SchedulerManager.Scheduler;
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="jobInfo">任务信息</param>
        /// <returns>是否创建成功</returns>
        public static async Task<bool> RunJobAsync(CustomerJobInfo jobInfo)
        {
            var assembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", jobInfo.Dllname));
            var type = assembly.GetType(jobInfo.FullJobName);
            JobKey jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            if (!await _scheduler.CheckExists(jobKey))
            {
                IJobDetail job = JobBuilder.Create(type).WithIdentity(jobKey)
                    .UsingJobData(CreateJobDataMap("jobId", jobInfo.Id))
                    .UsingJobData(CreateJobDataMap("requestUrl", jobInfo.RequestUrl))
                    .Build();

                CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(jobInfo.Cron);
                ITrigger trigger =
                    TriggerBuilder.Create().StartNow().WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                        .ForJob(jobKey)
                        .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                        .Build();
                #region Quartz 任务miss之后三种操作
                /*
                             withMisfireHandlingInstructionDoNothing
                ——不触发立即执行
                ——等待下次Cron触发频率到达时刻开始按照Cron频率依次执行

                withMisfireHandlingInstructionIgnoreMisfires
                ——以错过的第一个频率时间立刻开始执行
                ——重做错过的所有频率周期后
                ——当下一次触发频率发生时间大于当前时间后，再按照正常的Cron频率依次执行

                withMisfireHandlingInstructionFireAndProceed
                ——以当前时间为触发频率立刻触发一次执行
                ——然后按照Cron频率依次执行*/
                #endregion

                await _scheduler.ScheduleJob(job, trigger);
            }
            return true;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobInfo">任务信息</param>
        /// <returns>是否删除成功</returns>
        public static async Task<bool> DeleteJobAsync(CustomerJobInfo jobInfo)
        {
            var jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            var triggerKey = CreateTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
            await _scheduler.PauseTrigger(triggerKey);
            bool tjob = await _scheduler.UnscheduleJob(triggerKey);
            bool djob = await _scheduler.DeleteJob(jobKey);
            return tjob == djob;
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="jobInfo">任务信息</param>
        /// <returns>是否暂停成功</returns>
        public static async Task<bool> PauseJobAsync(CustomerJobInfo jobInfo)
        {
            var jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            await _scheduler.PauseJob(jobKey);
            return true;
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="jobInfo">任务信息</param>
        /// <returns>是否回复成功</returns>
        public static async Task<bool> ResumeJob(CustomerJobInfo jobInfo)
        {
            var jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            await _scheduler.ResumeJob(jobKey);
            return true;
        }

        /// <summary>
        /// 更改任务运行周期
        /// </summary>
        /// <param name="jobInfo">任务信息</param>
        /// <returns>是否更改成功</returns>
        public static async Task<bool> ModifyJobCronAsync(CustomerJobInfo jobInfo)
        {
            var scheduleBuilder = CronScheduleBuilder.CronSchedule(jobInfo.Cron);
            var triggerKey = CreateTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
            var trigger = TriggerBuilder.Create().StartNow().WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                .Build();
            await _scheduler.RescheduleJob(triggerKey, trigger);
            return true;
        }

        /// <summary>
        /// 获取耽搁任务状态（从Scheduler获取）
        /// </summary>
        /// <param name="triggerName">任务名称</param>
        /// <param name="triggerGroupName">任务组名称</param>
        /// <returns></returns>
        private static async Task<TriggerState> GetTriggerStateAsync(string triggerName, string triggerGroupName)
        {
            var triggerKey = CreateTriggerKey(triggerName, triggerGroupName);
            var triggerState = await _scheduler.GetTriggerState(triggerKey);
            return triggerState;
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="customerJobInfoList">任务原始列表集合</param>
        /// <param name="jobStatus">任务当前状态</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>任务列表</returns>
        public static async Task<object> GetJobListAsync(List<CustomerJobInfo> customerJobInfoList, int jobStatus, int pageIndex, int pageSize)
        {
            var allJobList = customerJobInfoList.Select(i => new
            {
                i.Id,
                i.JobName,
                i.JobGroupName,
                i.TriggerName,
                i.TriggerGroupName,
                i.Description,
                i.Cron,
                JobStartTime = i.JobStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = i.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                i.Deleted,
                Customer_TriggerState = i.TriggerState,
                TriggerState = ChangeState(GetTriggerStateAsync(i.TriggerName, i.TriggerGroupName).Result)
            }).ToList();
            allJobList = jobStatus == 5 || jobStatus == -1 ? allJobList.Where(i => i.Customer_TriggerState == jobStatus).ToList() : allJobList.Where(x => x.TriggerState == jobStatus).ToList();

            return await Task.Run(() => allJobList.Select(i => new
            {
                i.Id,
                i.JobName,
                i.JobGroupName,
                i.TriggerName,
                i.TriggerGroupName,
                i.Description,
                i.Cron,
                i.JobStartTime
            }))
            ;
        }

        private static JobKey CreateJobKey(string jobName, string jobGroupName)
        {
            return new JobKey(jobName, jobGroupName);
        }

        private static TriggerKey CreateTriggerKey(string triggerName, string triggerGroupName)
        {
            return new TriggerKey(triggerName, triggerGroupName);
        }

        private static int ChangeState(TriggerState triggerState)
        {
            switch (triggerState)
            {
                case TriggerState.None:
                    return -1;
                case TriggerState.Normal:
                    return 0;
                case TriggerState.Paused:
                    return 1;
                case TriggerState.Complete:
                    return 2;
                case TriggerState.Error:
                    return 3;
                case TriggerState.Blocked:
                    return 4;
                default:
                    return -1;
            }
        }

        private static JobDataMap CreateJobDataMap<T>(string propertyName, T propertyValue)
        {
            return new JobDataMap(new Dictionary<string, T> { { propertyName, propertyValue } });
        }
    }
}
