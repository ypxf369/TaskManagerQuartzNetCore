using System;
using System.Collections.Generic;

namespace QuartzNetCore.EFModel_SqlServer.Models
{
    public partial class CustomerJobInfo
    {
        public int Id { get; set; }
        public string JobGroupName { get; set; }
        public string JobName { get; set; }
        public string TriggerName { get; set; }
        public string Cron { get; set; }
        public int TriggerState { get; set; }
        public DateTime JobStartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? PreTime { get; set; }
        public DateTime? NextTime { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public string TriggerGroupName { get; set; }
        public string Dllname { get; set; }
        public string FullJobName { get; set; }
        public bool Deleted { get; set; }
        public string Exception { get; set; }
        public string RequestUrl { get; set; }
    }
}
