using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.CRMs.Models {
    public class CRMMaintenanceLogEntry {
        [Key]
        public int EntryId { get; set; }

        public DateTime EntryDate { get; set; }
        public string MaintenanceDescription { get; set; }
        public string ActionsTaken { get; set; }
        public string LastUpdatedBy { get; set; }

        public ContinuousRadonMonitor CRMId { get; set; }
    }
}