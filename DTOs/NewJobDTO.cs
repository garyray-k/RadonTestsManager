using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.Model {
    public class NewJobDTO {
        [Required]
        int JobNumber { get; set; }
        [Required]
        [MinLength(3)]
        public string ServiceType { get; set; }
        public Address JobAddress { get; set; }
        public DateTime ServiceDeadLine { get; set; }
        public string DeviceType { get; set; }
        public string AccessInfo { get; set; }
        public string SpecialNotes { get; set; }
        public string Driver { get; set; }
        public DateTime ArrivalTime { get; set; }       
    }
}
