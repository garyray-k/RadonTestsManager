using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.DTOs {
    public class AddressDTO {
        [Required]
        public int AddressId { get; set; }
        [MaxLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [MaxLength(200)]
        public string Address1 { get; set; }
        [MaxLength(200)]
        public string Address2 { get; set; }
        [MaxLength(60)]
        public string City { get; set; }
        [MaxLength(60)]
        public string Country { get; set; }
        [Required]
        [MaxLength(5)]
        [MinLength(5)]
        public string PostalCode { get; set; }
        [MaxLength(2)]
        [MinLength(2)]
        public string State { get; set; }
    }
}
