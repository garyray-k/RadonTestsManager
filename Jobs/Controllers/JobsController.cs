using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadonTestsManager.DBContext;
using RadonTestsManager.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : Controller {
        // GET: api/values
        [HttpGet]
        [HttpGet("{jobid:int?}")]
        public string Get(int jobid) {
            string[] jobs = new[] {
                "1 - JobNumber1",
                "2 - JobNumber2",
                "3 - JobNumber3"
            };
            return jobs[jobid + 1];
        }

        // GET api/values/5
        [HttpGet("search/{jobid}/{category=all}/")]
        public string[] SearchByJobCategory(int id, string category, string location="all") {
            return new[] {
                $"Job Id: {id}, Category: {category}, Location: {location}" 
                };
        }

        // POST api/values
        [HttpPost("")]
        public async Task<ActionResult<NewJobDTO>> AddNewJob([FromBody] NewJobDTO newJob) {
            await Task.Delay(500);//not needed
            return newJob;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value) {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }

    }
}
