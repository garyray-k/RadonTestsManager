﻿using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.Model {
    public class NewLSVialDTO {
        [Required]
        public int SerialNumber { get; set; }
        public string Status { get; set; }
        [Required]
        public DateTime TestStart { get; set; }
        public DateTime? TestFinish { get; set; }
        [Required]
        public int JobNumber { get; set; }
    }
}