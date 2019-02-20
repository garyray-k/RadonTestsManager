using System;
using System.ComponentModel.DataAnnotations;

namespace RadonTestsManager.DTOs {
    public class LoginUserDTO {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
