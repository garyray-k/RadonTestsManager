using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.DTOs {
    public class CRMMaintenanceLogEntryDTO {
        [Required]
        [Range(0, 99999999)]
        public int EntryId;
        public DateTime EntryDate;
        [MaxLength(255)]
        public string MaintenanceDescription;
        [MaxLength(512)]
        public string ActionsTaken;
    }
}
