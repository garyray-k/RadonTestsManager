using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadonTestsManager.Models {
    public class LSVial {
        public int LSVialId { get; set; }

        public int SerialNumber{ get; set; }
        public string Status{ get; set; }
        public DateTime TestStart{ get; set; }
        public DateTime TestFinish{ get; set; }
        public string LastUpdatedBy { get; set; }

        public virtual Job Job { get; set; }
    }
}
