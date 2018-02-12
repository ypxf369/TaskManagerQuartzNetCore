using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuartzNetCore.EFModel_SqlServer.Models;
using QuartzNetCore.IRepository;

namespace QuartzNetCore.Repository
{
    public class CustomerJobInfoRepository : ICustomerJobInfoRepository
    {
        private readonly QuartzManagerContext _dbContext;

        public CustomerJobInfoRepository()
        {
            _dbContext = DbContextFactory.DbContext;
        }
        public async Task<int> AddCustomerJobInfoAsync(string jobName, string jobGroupName, string triggerName, string triggerGroupName, string cron, string jobDescription, string requestUrl)
        {
            var customerJobInfo = new CustomerJobInfo
            {
                CreateTime = DateTime.Now,
                JobStartTime = DateTime.Now,
                Cron = cron,
                Description = jobDescription,
                JobGroupName = jobGroupName,
                JobName = jobName,
                TriggerState = -1,
                TriggerGroupName = triggerGroupName,
                TriggerName = triggerName,
                Dllname = "Quartz.Net_JobBase.dll",
                FullJobName = "Quartz.Net_JobBase.JobBase",
                RequestUrl = requestUrl,
                Deleted = false
            };
            var model = await _dbContext.CustomerJobInfo.AddAsync(customerJobInfo);
            await _dbContext.SaveChangesAsync();
            return model.Entity.Id;
        }

        public async Task<Tuple<IQueryable<CustomerJobInfo>, int>> LoadCustomerInfoesAsync<T>(Expression<Func<CustomerJobInfo, bool>> whereLambda, Expression<Func<CustomerJobInfo, T>> orderByLambda, bool isAsc, int pageIndex, int pageSize)
        {
            int totalCount;
            var cstmJobInfos = _dbContext.CustomerJobInfo.Where(whereLambda);
            totalCount = await cstmJobInfos.CountAsync();
            var data = isAsc
                //? cstmJobInfos.OrderBy(orderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                //: cstmJobInfos.OrderByDescending(orderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                ? cstmJobInfos.OrderBy(orderByLambda)
                : cstmJobInfos.OrderByDescending(orderByLambda);
            return await Task.FromResult(new Tuple<IQueryable<CustomerJobInfo>, int>(data, totalCount));
        }

        public async Task<int> UpdateCustomerJobInfoAsync(CustomerJobInfo customerJobInfo)
        {
            await _dbContext.SaveChangesAsync();
            return customerJobInfo.Id;
        }

        public async Task<CustomerJobInfo> LoadCustomerJobInfoAsync(Expression<Func<CustomerJobInfo, bool>> whereLambda)
        {
            return await _dbContext.CustomerJobInfo.SingleOrDefaultAsync(whereLambda);
        }
    }
}
