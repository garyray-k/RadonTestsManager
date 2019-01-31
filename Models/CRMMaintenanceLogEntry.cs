using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.CRMs.Models {
    public class CRMMaintenanceLogEntry {
        [Key]
        public int EntryId { get; set; }

        public DateTime EntryDate;
        public string MaintenanceDescription;
        public string ActionsTaken;
        public string LastUpdatedBy { get; set; }

        public ContinuousRadonMonitor CRMId { get; set; }
    }
}