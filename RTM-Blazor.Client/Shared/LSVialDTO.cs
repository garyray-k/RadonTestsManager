using System;
using RTM_Blazor.Client.Shared;

namespace RTM_Blazor.Client.Shared {
    public class LSVialDTO {
        public int LSVialId { get; set; }
        public int SerialNumber { get; set; }
        public string Status { get; set; }
        public DateTime TestStart { get; set; }
        public DateTime? TestFinish { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}