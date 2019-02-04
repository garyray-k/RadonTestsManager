using System;
namespace RadonTestsManager.DTOs {
    public class CRMMaintenanceLogEntryDTO {
        public DateTime EntryDate;
        public string MaintenanceDescription;
        public string ActionsTaken;
        public string LastUpdatedBy { get; set; }
    }
}
