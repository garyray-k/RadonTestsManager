using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.Controllers {
    public class ContinuousRadonMonitorDTO {
        [Required]
        public int MonitorNumber { get; set; }
        [Required]
        public int SerialNumber { get; set; }
        public DateTime LastCalibrationDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime LastBatteryChangeDate { get; set; }
        public DateTime TestStart { get; set; }
        public DateTime? TestFinish { get; set; }
        public string Status { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}