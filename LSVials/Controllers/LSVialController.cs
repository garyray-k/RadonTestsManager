using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadonTestsManager.DBContext;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.LSVials.Models;
using RadonTestsManager.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.LSVials.Controllers {
    [Route("api/[controller]")]
    public class LSVialController : Controller {
        private readonly RadonTestsManagerContext _context;

        public LSVialController(RadonTestsManagerContext context) {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get() {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id) {
            return "value";
        }

        // POST api/values
        [HttpPost("")]
        public async Task<IActionResult> AddNewLSVial([FromBody] NewLSVialDTO newLSVial) {
            var lSVial = new LSVial() {
                SerialNumber = newLSVial.SerialNumber,
                Status = newLSVial.Status,
                TestStart = newLSVial.TestStart,
                TestFinish = newLSVial.TestStart.AddDays(2),
                JobHistory = new List<Job> { }
            };
            //TODO check for existence of job
            var newJob = new Job() { JobNumber = newLSVial.JobNumber };
            lSVial.JobHistory.Add(newJob);
            _context.LSVials.Add(lSVial);
            _context.Jobs.Add(newJob);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(Get),
                new { id = lSVial.LSVialId, name = "ThisIsTheName", status = (lSVial.Status + "updated") });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLSVialStatus(int id, [FromBody]string value) {
            var lSVial = await _context.LSVials.FindAsync(id);
            lSVial.Status = value;
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(PutLSVialStatus),
                new { id = id, status = ("This is my new status: " + value) });

        }

        // DELETE api/values/5
        // TODO need to delete Job from DB first before removing vial (FK constraint)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLSVial(int id) {
            var lSVial = await _context.LSVials.FindAsync(id);
            _context.LSVials.Remove(lSVial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Get));
        }
    }
}
