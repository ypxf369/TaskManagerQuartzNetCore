using System;
using System.Collections.Generic;

namespace QuartzNetCore.EFModel_SqlServer.Models
{
    public partial class QrtzBlobTriggers
    {
        public string SchedName { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public byte[] BlobData { get; set; }
    }
}
