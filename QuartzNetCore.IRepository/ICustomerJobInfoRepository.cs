using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using QuartzNetCore.EFModel_SqlServer.Models;

namespace QuartzNetCore.IRepository
{
    public interface ICustomerJobInfoRepository
    {
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务所在组名称</param>
        /// <param name="triggerName">触发器名称</param>
        /// <param name="triggerGroupName">触发器所在组名称</param>
        /// <param name="cron">执行周期表达式</param>
        /// <param name="jobDescription">任务描述</param>
        /// <param name="requestUrl">请求地址</param>
        /// <returns>添加后任务编号</returns>
        Task<int> AddCustomerJobInfoAsync(string jobName, string jobGroupName, string triggerName,
            string triggerGroupName, string cron, string jobDescription, string requestUrl);

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="customerJobInfo">任务model</param>
        /// <returns>任务编号</returns>
        Task<int> UpdateCustomerJobInfo(CustomerJobInfo customerJobInfo);

        /// <summary>
        /// 加载任务列表
        /// </summary>
        /// <typeparam name="T">排序表达式返回的值类型</typeparam>
        /// <param name="whereLambda">条件表达式</param>
        /// <param name="orderByLambda">排序表达式</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>数据集合</returns>
        Task<Tuple<IQueryable<CustomerJobInfo>, int>> LoadCustomerInfoes<T>(
            Expression<Func<CustomerJobInfo, bool>> whereLambda, Expression<Func<CustomerJobInfo, T>> orderByLambda,
            bool isAsc, int pageIndex, int pageSize);

        /// <summary>
        /// 加载单个任务
        /// </summary>
        /// <param name="whereLambda">条件表达式</param>
        /// <returns>单个任务</returns>
        Task<CustomerJobInfo> LoadCustomerJobInfoAsync(Expression<Func<CustomerJobInfo, bool>> whereLambda);
    }
}
