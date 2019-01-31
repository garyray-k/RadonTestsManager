using System;
using RadonTestsManager.CRMs.Models;
using RadonTestsManager.LSVials.Models;
using RadonTestsManager.Model;

namespace RadonTestsManager.Jobs.Models {
    public class Job {
        public int JobId { get; set; }

        public int JobNumber { get; set; }
        public string ServiceType { get; set; }
        public DateTime ServiceDeadLine { get; set; }
        public string DeviceType { get; set; }
        public string AccessInfo { get; set; }
        public string SpecialNotes { get; set; }
        public string Driver { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string LastUpdatedBy { get; set; }

        public ContinuousRadonMonitor ContinousRadonMonitor{ get; set; }
        public LSVial LSvial { get; set; }
        public Address JobAddress { get; set; }
    }
}
