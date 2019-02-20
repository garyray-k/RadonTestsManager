﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RTM.Server.Utility;

namespace RadonTestsManager.DTOs {
    public class ContinuousRadonMonitorDTO {
        [Required]
        [Range(0, 99999999)]
        public int MonitorNumber { get; set; }
        [Required]
        [Range(0, 99999999)]
        public int SerialNumber { get; set; }
        public DateTime LastCalibrationDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime LastBatteryChangeDate { get; set; }
        public DateTime TestStart { get; set; }
        public DateTime? TestFinish { get; set; }
        [MaxLength(255)]
        public string Status { get; set; }
        public AddressDTO Addres { get; set; }
        public List<CRMMaintenanceLogEntryDTO> MaintenanceLog { get; set; }
        public List<int> JobHistory { get; set; }
    }
}