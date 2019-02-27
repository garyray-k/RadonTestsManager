using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadonTestsManager.Models {
    public class Job {
        public int JobId { get; set; }

        public int JobNumber { get; set; }
        public string ServiceType { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime ServiceDeadLine { get; set; }
        public string DeviceType { get; set; }
        public string AccessInfo { get; set; }
        public string SpecialNotes { get; set; }
        public string Driver { get; set; }
        public string TimeOfDay { get; set; }
        public DateTime ArrivalTime { get; set; }
        public bool Confirmed { get; set; }
        public bool Completed { get; set; }
        public string LastUpdatedBy { get; set; }

        [ForeignKey("CRMId")]
        public virtual ContinuousRadonMonitor ContinousRadonMonitor { get; set; }
        [ForeignKey("LSVialId")]
        public virtual LSVial LSvial { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
}
