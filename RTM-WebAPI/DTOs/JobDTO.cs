using System;
using System.ComponentModel.DataAnnotations;
using RadonTestsManager.DTOs;

namespace RadonTestsManager.DTOs {
    public class JobDTO {
        public int JobId { get; set; }
        [Required]
        [Range(0, 99999999)]
        public int JobNumber { get; set; }
        [Required]
        [MinLength(3)]
        public string ServiceType { get; set; }
        public DateTime ServiceDate { get; set; }
        public DateTime ServiceDeadLine { get; set; }
        [MaxLength(10)]
        public string DeviceType { get; set; }
        [MaxLength(255)]
        public string AccessInfo { get; set; }
        [MaxLength(512)]
        public string SpecialNotes { get; set; }
        [MaxLength(20)]
        public string Driver { get; set; }
        public string TimeOfDay { get; set; }
        public DateTime ArrivalTime { get; set; }
        public bool Confirmed { get; set; }
        public bool Completed { get; set; }
        public string LastUpdatedBy { get; set; }
        public int CRMId { get; set; }
        public int LSVialId { get; set; }
        public int AddressId { get; set; }
    }
}
