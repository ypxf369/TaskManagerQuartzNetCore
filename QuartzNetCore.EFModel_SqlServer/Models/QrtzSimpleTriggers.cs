using System;
using System.Collections.Generic;

namespace QuartzNetCore.EFModel_SqlServer.Models
{
    public partial class QrtzSimpleTriggers
    {
        public string SchedName { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public int RepeatCount { get; set; }
        public long RepeatInterval { get; set; }
        public int TimesTriggered { get; set; }

        public QrtzTriggers QrtzTriggers { get; set; }
    }
}
