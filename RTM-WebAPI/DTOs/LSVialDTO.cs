using System;
using System.ComponentModel.DataAnnotations;
using RadonTestsManager.Models;

namespace RadonTestsManager.DTOs {
    public class LSVialDTO {
        public int LSVialId { get; set; }
        [Required]
        [Range(0,99999999)]
        public int SerialNumber { get; set; }
        [MaxLength(255)]
        public string Status { get; set; }
        [Required]
        public DateTime TestStart { get; set; }
        public DateTime? TestFinish { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
