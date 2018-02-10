using System;
using System.Collections.Generic;

namespace QuartzNetCore.EFModel_SqlServer.Models
{
    public partial class QrtzLocks
    {
        public string SchedName { get; set; }
        public string LockName { get; set; }
    }
}
