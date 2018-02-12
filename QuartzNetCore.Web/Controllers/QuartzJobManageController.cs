using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuartzNetCore.EFModel_SqlServer.Models;
using QuartzNetCore.IRepository;
using QuartzNetCore.Web.Factories;
using QuartzNetCore.Web.Models;
using QuartzNetCore.Web.QuartzNetSchedulerManager;

namespace QuartzNetCore.Web.Controllers
{
    public class QuartzJobManageController : Controller
    {
        public ICustomerJobInfoRepository _customerJobInfoRepository;

        public QuartzJobManageController(ICustomerJobInfoRepository customerJobInfoRepository)
        {
            _customerJobInfoRepository = customerJobInfoRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务组名称</param>
        /// <param name="triggerName">触发器名称</param>
        /// <param name="triggerGroupName">触发器组名称</param>
        /// <param name="cron">执行周期表达式</param>
        /// <param name="jobDescription">任务描述</param>
        /// <param name="requestUrl">请求地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddJob(string jobName, string jobGroupName, string triggerName,
            string triggerGroupName, string cron, string jobDescription, string requestUrl)
        {
            int jobId = await _customerJobInfoRepository.AddCustomerJobInfoAsync(jobName, jobGroupName, triggerName,
                triggerGroupName, cron, jobDescription, requestUrl);
            return await Task.FromResult(Json(ResponseDataFactory.CreateAjaxResponseData("1", "添加成功", jobId)));
        }

        /// <summary>
        /// 更改任务执行周期（任务运行中）
        /// </summary>
        /// <param name="jobId">任务编号</param>
        /// <param name="cron">周期表达式</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ModefyJobCron(int jobId, string cron)
        {
            var ajaxResponseData = OperateJobAsync(jobId, jobDetail =>
            {
                jobDetail.Cron = cron;
                _customerJobInfoRepository.UpdateCustomerJobInfoAsync(jobDetail);
                return JobHelper.ModifyJobCronAsync(jobDetail).Result;
            });
            return await Task.FromResult(Json(ajaxResponseData));
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="jobId">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RunJob(int jobId)
        {
            var ajaxResponseData = OperateJobAsync(jobId, jobDetail =>
            {
                jobDetail.TriggerState = 0;
                _customerJobInfoRepository.UpdateCustomerJobInfoAsync(jobDetail);
                return JobHelper.RunJobAsync(jobDetail).Result;
            });
            return await Task.FromResult(Json(ajaxResponseData));
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobId">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteJob(int jobId)
        {
            var ajaxResponseData = OperateJobAsync(jobId, jobDetail =>
            {
                jobDetail.Deleted = true;
                jobDetail.TriggerState = 5;
                _customerJobInfoRepository.UpdateCustomerJobInfoAsync(jobDetail);
                return JobHelper.DeleteJobAsync(jobDetail).Result;
            });
            return await Task.FromResult(Json(ajaxResponseData));
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="jobId">任务编号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> PauseJob(int jobId)
        {
            var ajaxResponseData = OperateJobAsync(jobId, jobDetail =>
            {
                jobDetail.TriggerState = 1;
                _customerJobInfoRepository.UpdateCustomerJobInfoAsync(jobDetail);
                return JobHelper.PauseJobAsync(jobDetail).Result;
            });
            return await Task.FromResult(Json(ajaxResponseData));
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="jobId">任务编号</param>
        /// <returns></returns>
        public async Task<JsonResult> ResumeJob(int jobId)
        {
            var ajaxResponseData = OperateJobAsync(jobId, jobDetail =>
            {
                jobDetail.TriggerState = 0;
                _customerJobInfoRepository.UpdateCustomerJobInfoAsync(jobDetail);
                return JobHelper.ResumeJob(jobDetail).Result;
            });
            return await Task.FromResult(Json(ajaxResponseData));
        }

        public async Task<JsonResult> GetJobList(int jobStatus, int pageIndex, int pageSize)
        {
            var jobQueryable = await _customerJobInfoRepository.LoadCustomerInfoesAsync(i => i.TriggerState == jobStatus,
                i => i.Id, false, pageIndex, pageSize);
            var jobs = await jobQueryable.Item1.ToListAsync();
            var jobList = jobs.Select(i => new
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
                i.Exception
            }).ToList();
            var
            ajaxResponseData = ResponseDataFactory.CreateAjaxResponseData("1", "获取成功", new { jobList, TotalCount = jobQueryable.Item2 });
            return await Task.FromResult(Json(ajaxResponseData));
        }

        private async Task<AjaxResponseData> OperateJobAsync(int jobId, Func<CustomerJobInfo, bool> operateJobFunc)
        {
            AjaxResponseData ajaxResponseData = null;
            var jobDetail = await _customerJobInfoRepository.LoadCustomerJobInfoAsync(i => i.Id == jobId);
            if (jobDetail == null)
            {
                ajaxResponseData = ResponseDataFactory.CreateAjaxResponseData("0", "无此任务", jobDetail);
            }
            else
            {
                var isSuccess = operateJobFunc(jobDetail);
                if (isSuccess)
                {
                    ajaxResponseData = ResponseDataFactory.CreateAjaxResponseData("1", "操作成功", jobDetail);
                }
                else
                {
                    ajaxResponseData = ResponseDataFactory.CreateAjaxResponseData("-10001", "操作失败", jobDetail);
                }
            }
            return ajaxResponseData;
        }
    }
}