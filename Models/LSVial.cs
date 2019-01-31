using System;
using System.Collections.Generic;
using RadonTestsManager.Jobs.Models;

namespace RadonTestsManager.LSVials.Models {
    public class LSVial {
        public int LSVialId { get; set; }

        public int SerialNumber{ get; set; }
        public string Status{ get; set; }
        public DateTime TestStart{ get; set; }
        public DateTime TestFinish{ get; set; }
        public string LastUpdatedBy { get; set; }

        public List<Job> JobHistory { get; set; }

        public LSVial() {
        }
    }
}
