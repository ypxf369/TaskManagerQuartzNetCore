using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace QuartzNetCore.JobBase
{
    public class JobBase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                //HttpClient hc = new HttpClient();
                //hc.GetAsync(context.JobDetail.JobDataMap["requestUrl"].ToString());
                Random r = new Random();
                int n = r.Next(0, 9999);
                await System.IO.File.WriteAllTextAsync($"G://{n}.txt", n.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
