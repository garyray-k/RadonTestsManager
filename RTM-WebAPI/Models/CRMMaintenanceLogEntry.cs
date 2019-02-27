using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadonTestsManager.Models {
    public class CRMMaintenanceLogEntry {
        [Key]
        public int EntryId { get; set; }

        public DateTime EntryDate { get; set; }
        public string MaintenanceDescription { get; set; }
        public string ActionsTaken { get; set; }
        public string LastUpdatedBy { get; set; }

        [ForeignKey("CRMId")]
        public virtual ContinuousRadonMonitor ContinuousRadonMonitor { get; set; }
    }
}