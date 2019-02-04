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
    [Authorize]
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

        // GET: api/lsvial
        [HttpGet]
        public async Task<ActionResult<LSVialDTO[]>> GetAllLSVials() {
            List<LSVial> lSVials = await _context.LSVials.ToListAsync();
            return lSVials == null ? (ActionResult<LSVialDTO[]>)NotFound() : (ActionResult<LSVialDTO[]>)Ok(_lsVialMapper.Map<LSVialDTO[]>(lSVials));
        }

        // GET api/lsvial/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LSVialDTO>> GetLSVialBySerialNumber(int serialNum) {
            var lsVial = await _context.LSVials
                    .Include(p => p.LSVialId)
                    .Include(p => p.Status)
                    .FirstOrDefaultAsync(p => p.SerialNumber == serialNum);
            if (lsVial == null) {
                return NotFound();
            }

            return Ok(_lsVialMapper.Map<LSVialDTO>(lsVial));
        }

        // POST api/lsvial
        [HttpPost("")]
        public async Task<IActionResult> AddNewLSVial([FromBody] LSVialDTO newLSVial) {
            if (await _context.LSVials.AnyAsync(x => x.SerialNumber.Equals(newLSVial.SerialNumber))) {
                return BadRequest("Error: A LS Vial already exists with that Serial Number.");
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);
            var lSVial = new LSVial() {
                SerialNumber = newLSVial.SerialNumber,
                Status = newLSVial.Status,
                TestStart = newLSVial.TestStart,
                TestFinish = newLSVial.TestStart.AddDays(2),
                JobHistory = new List<Job> { },
                LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString()
            };

            Job job;
            bool jobExists = await _context.Jobs.AnyAsync(x => x.JobNumber.Equals(newLSVial.JobNumber));
            if (jobExists) {
                job = await _context.Jobs.FirstOrDefaultAsync(y => y.JobNumber == newLSVial.JobNumber);
            } else {
                job = new Job() { JobNumber = newLSVial.JobNumber };
                await _context.Jobs.AddAsync(job);
            }

            lSVial.JobHistory.Add(job);
            await _context.LSVials.AddAsync(lSVial);

            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetLSVialBySerialNumber),
                new { sn = lSVial.SerialNumber, jobNum = job.JobNumber, user, dateCreated = DateTime.UtcNow});
        }

        // PUT api/lsvial/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLSVialWithoutJobAddition(int id, [FromBody]LSVialDTO updatedLSVial) {
            var lSVial = await _context.LSVials.FindAsync(id);
            lSVial.SerialNumber = updatedLSVial.SerialNumber;
            lSVial.Status = updatedLSVial.Status;
            lSVial.TestStart = updatedLSVial.TestStart;
            lSVial.TestFinish = updatedLSVial.TestStart.AddDays(2);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            lSVial.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateLSVialWithoutJobAddition),
                _lsVialMapper.Map<LSVialDTO>(lSVial));

        }

        [HttpPut("addjob/{id}")]
        public async Task<IActionResult> AddJobToLSVial(int id, [FromBody]JobDTO job) {
            var lSVial = await _context.LSVials.FindAsync(id);
            lSVial.JobHistory.Add(job);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(AddJobToLSVial),
                new { sn = lSVial.SerialNumber, jobAdded = job });
        }

        // DELETE api/lsvial/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLSVial(int id) {
            var lSVial = await _context.LSVials.FindAsync(id);
            lSVial.JobHistory.Clear();
            _context.LSVials.Remove(lSVial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllLSVials));
        }
    }
}
