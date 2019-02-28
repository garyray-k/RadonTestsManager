using System;
using RTM_Blazor.Client.Shared;

namespace RTM_Blazor.Client.Shared {
    public class ContinuousRadonMonitorDTO {
        public int CrmId { get; set; }
        public int MonitorNumber { get; set; }
        public int SerialNumber { get; set; }
        public DateTime LastCalibrationDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime LastBatteryChangeDate { get; set; }
        public DateTime TestStart { get; set; }
        public DateTime? TestFinish { get; set; }
        public string Status { get; set; }
        public int AddressId { get; set; }
        public int[] MaintenanceLog { get; set; }
        public int[] JobHistory { get; set; }
    }
}