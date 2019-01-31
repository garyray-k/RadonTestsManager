using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.Model;

namespace RadonTestsManager.CRMs.Models {
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

        public Address Location{  get; set; }
        public List<CRMMaintenanceLogEntry> MaintenanceLog{  get; set; }
        public List<Job> JobHistory { get; set; }

        public ContinuousRadonMonitor() {
        }
    }
}
