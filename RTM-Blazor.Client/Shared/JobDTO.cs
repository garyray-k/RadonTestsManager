using System;
using RTM_Blazor.Client.Shared;

namespace RTM_Blazor.Client.Shared {
    public class JobDTO {
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
        public AddressDTO JobAddress { get; set; }
    }
}