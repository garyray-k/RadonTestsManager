using System;
using Microsoft.AspNetCore.Identity;

namespace RadonTestsManager.Users {
    public class User : IdentityUser {
        public int EmployeeNumber { get; set; }
        public string Company { get; set; }
        public string EmployeeRole { get; set; }

    }
}
