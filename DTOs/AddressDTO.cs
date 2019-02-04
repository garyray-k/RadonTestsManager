using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.DTOs {
    public class AddressDTO {
        public int AddressId { get; set; }
        public string CustomerName { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [Required]
        public string PostalCode { get; set; }
        public string State { get; set; }
    }
}
