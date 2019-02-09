using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.Model {
    public class JobDTO {
        [Required]
        [Range(0, 99999999)]
        public int JobNumber { get; set; }
        [Required]
        [MinLength(3)]
        public string ServiceType { get; set; }
        public Address JobAddress { get; set; }
        public DateTime ServiceDeadLine { get; set; }
        [MaxLength(10)]
        public string DeviceType { get; set; }
        [MaxLength(255)]
        public string AccessInfo { get; set; }
        [MaxLength(512)]
        public string SpecialNotes { get; set; }
        [MaxLength(20)]
        public string Driver { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
