using System;

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

        public int ContinuosRadonMonitorId { get; set; }
        public ContinuousRadonMonitor ContinousRadonMonitor{ get; set; }

        public int LSvialId { get; set; }
        public LSVial LSvial { get; set; }

        public int JobAddressId { get; set; }
        public Address JobAddress { get; set; }
    }
}
