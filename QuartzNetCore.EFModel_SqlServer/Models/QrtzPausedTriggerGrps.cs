using System;
using System.Collections.Generic;

namespace QuartzNetCore.EFModel_SqlServer.Models
{
    public partial class QrtzPausedTriggerGrps
    {
        public string SchedName { get; set; }
        public string TriggerGroup { get; set; }
    }
}
