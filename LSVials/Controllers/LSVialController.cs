using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RadonTestsManager.DBContext;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.LSVials.Models;
using RadonTestsManager.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.LSVials.Controllers {
    [Route("api/[controller]")]
    public class LSVialController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _lsVialMapper;
        private readonly ILogger<LSVialController> _logger;

        static LSVialController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<LSVial, LSVialDTO>()
                    .ForMember(dto => dto.SerialNumber, opt => opt.MapFrom(lsvial => lsvial.SerialNumber))
                    .ForMember(dto => dto.Status, opt => opt.MapFrom(lsvial => lsvial.Status))
                    .ForMember(dto => dto.TestStart, opt => opt.MapFrom(lsvial => lsvial.TestStart))
                    .ForMember(dto => dto.TestFinish, opt => opt.MapFrom(lsvial => lsvial.TestFinish));
            });

            _lsVialMapper = config.CreateMapper();
        }

        public LSVialController(RadonTestsManagerContext context, ILogger<LSVialController> logger) {
            _context = context;
            _logger = logger;
        }

        // GET: api/values
        // TODO removed but left for reference [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<LSVialDTO[]>> GetAllLSVials() {
            List<LSVial> lSVials = await _context.LSVials
                .ToListAsync();
            return lSVials == null ? (ActionResult<LSVialDTO[]>)NotFound() : (ActionResult<LSVialDTO[]>)Ok(_lsVialMapper.Map<LSVialDTO[]>(lSVials));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LSVialDTO>> GetLSVial(int serialNum) {
            var lsVial = await _context.LSVials
                    .Include(p => p.LSVialId)
                    .Include(p => p.Status)
                    .FirstOrDefaultAsync(p => p.SerialNumber == serialNum);
            if (lsVial == null) {
                return NotFound();
            }

            return Ok(_lsVialMapper.Map<LSVialDTO>(lsVial));
        }

        // POST api/values
        [Authorize]
        [HttpPost("")]
        public async Task<IActionResult> AddNewLSVial([FromBody] LSVialDTO newLSVial) {
            var user = await _context.Users.FindAsync(User.Identity.Name);

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
                nameof(GetLSVial),
                new { id = lSVial.LSVialId, name = "ThisIsTheName", status = (lSVial.Status + "updated")});
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
            return RedirectToAction(nameof(GetAllLSVials));
        }
    }
}
