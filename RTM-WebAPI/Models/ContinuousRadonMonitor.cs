using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadonTestsManager.Models {
    public class ContinuousRadonMonitor {
        [Key]
        public int CRMId { get; set; }

        public int MonitorNumber{  get; set; }
        public int SerialNumber{  get; set; }
        public DateTime LastCalibrationDate{  get; set; }
        public DateTime PurchaseDate{  get; set; }
        public DateTime LastBatteryChangeDate{  get; set; }
        public DateTime TestStart{  get; set; }
        public DateTime TestFinish{  get; set; }
        public string Status{  get; set; }
        public string LastUpdatedBy { get; set; }

        public  virtual Address Address{  get; set; }

        public virtual List<CRMMaintenanceLogEntry> MaintenanceLogHistory { get; private set; }

        public virtual List<Job> JobHistory { get; private set; }
    }
}
