using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.Utility {
    [Route("api/[controller]")]
    public class ErrorThrower : Controller {
        // GET: api/values
        [AllowAnonymous]
        [HttpGet("error")]
        public ActionResult ThrowError() {
            throw new InvalidOperationException("Example error intentionally generated.");
        }
    }
}
