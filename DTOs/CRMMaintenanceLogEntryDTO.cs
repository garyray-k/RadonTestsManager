using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.DTOs {
    public class CRMMaintenanceLogEntryDTO {
        [Required]
        public int EntryId;
        public DateTime EntryDate;
        public string MaintenanceDescription;
        public string ActionsTaken;
    }
}
