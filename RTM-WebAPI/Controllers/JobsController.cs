using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RadonTestsManager.DBContext;
using RadonTestsManager.DTOs;
using RadonTestsManager.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.Controllers {
    //[Authorize]
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class JobsController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _jobsMapper;
        private readonly ILogger<JobsController> _logger;

        static JobsController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Job, JobDTO>()
                    .ForMember(dto => dto.JobId, opt => opt.MapFrom(job => job.JobId))
                    .ForMember(dto => dto.JobNumber, opt => opt.MapFrom(job => job.JobNumber))
                    .ForMember(dto => dto.ServiceType, opt => opt.MapFrom(job => job.ServiceType))
                    .ForMember(dto => dto.ServiceDate, opt => opt.MapFrom(job => job.ServiceDate))
                    .ForMember(dto => dto.ServiceDeadLine, opt => opt.MapFrom(job => job.ServiceDeadLine))
                    .ForMember(dto => dto.DeviceType, opt => opt.MapFrom(job => job.DeviceType))
                    .ForMember(dto => dto.AccessInfo, opt => opt.MapFrom(job => job.AccessInfo))
                    .ForMember(dto => dto.SpecialNotes, opt => opt.MapFrom(job => job.SpecialNotes))
                    .ForMember(dto => dto.Driver, opt => opt.MapFrom(job => job.Driver))
                    .ForMember(dto => dto.TimeOfDay, opt => opt.MapFrom(job => job.TimeOfDay))
                    .ForMember(dto => dto.ArrivalTime, opt => opt.MapFrom(job => job.ArrivalTime))
                    .ForMember(dto => dto.Confirmed, opt => opt.MapFrom(job => job.Confirmed))
                    .ForMember(dto => dto.Completed, opt => opt.MapFrom(job => job.Completed));
                cfg.CreateMap<JobDTO, Job>()
                    .ForMember(job => job.JobId, opt => opt.MapFrom(dto => dto.JobId))
                    .ForMember(job => job.JobNumber, opt => opt.MapFrom(dto => dto.JobNumber))
                    .ForMember(job => job.ServiceType, opt => opt.MapFrom(dto => dto.ServiceType))
                    .ForMember(job => job.ServiceDate, opt => opt.MapFrom(dto => dto.ServiceDate))
                    .ForMember(job => job.ServiceDeadLine, opt => opt.MapFrom(dto => dto.ServiceDeadLine))
                    .ForMember(job => job.DeviceType, opt => opt.MapFrom(dto => dto.DeviceType))
                    .ForMember(job => job.AccessInfo, opt => opt.MapFrom(dto => dto.AccessInfo))
                    .ForMember(job => job.SpecialNotes, opt => opt.MapFrom(dto => dto.SpecialNotes))
                    .ForMember(job => job.Driver, opt => opt.MapFrom(dto => dto.Driver))
                    .ForMember(job => job.TimeOfDay, opt => opt.MapFrom(dto => dto.TimeOfDay))
                    .ForMember(job => job.ArrivalTime, opt => opt.MapFrom(dto => dto.ArrivalTime))
                    .ForMember(job => job.Confirmed, opt => opt.MapFrom(dto => dto.Confirmed))
                    .ForMember(job => job.Completed, opt => opt.MapFrom(dto => dto.Completed));
            });

            _jobsMapper = config.CreateMapper();
            }

        public JobsController(RadonTestsManagerContext context, ILogger<JobsController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<JobDTO[]>> GetAllJobs() {
            Job[] jobs = await _context.Jobs.ToArrayAsync();
            return jobs == null ? (ActionResult<JobDTO[]>)NotFound() : (ActionResult<JobDTO[]>)Ok(_jobsMapper.Map<JobDTO[]>(jobs));
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<JobDTO>> GetJobByNumber(int jobId) {
            var job = await _context.Jobs.FindAsync(jobId);
            return job == null ? (ActionResult<JobDTO>)NotFound() : (ActionResult<JobDTO>)Ok(_jobsMapper.Map<JobDTO>(job));
        }

        [HttpGet("address/{addressId}")]
        public async Task<ActionResult<int[]>> GetJobHistoryByAddressId(int addressId) {
            var address = await _context.Addresses.FindAsync(addressId);
            var jobs = await _context.Jobs.Where(x => x.Address == address).Select(x => x.JobNumber).ToArrayAsync();
            return jobs == null ? (ActionResult<int[]>)NotFound() : (ActionResult<int[]>)Ok(jobs);
        }

        [HttpPost("")]
        public async Task<ActionResult<JobDTO>> AddNewJob([FromBody] JobDTO newJob) {
            if (await _context.Jobs.AnyAsync(x => x.JobNumber.Equals(newJob.JobNumber))) {
                _logger.LogWarning("An existing Job was sent to AddNewJob.");
                return BadRequest("Error: A Job already exists with that Job Number.");
            }
            if (newJob.DeviceType != "LS Vial" || newJob.DeviceType != "CRM" || newJob.DeviceType != "Unknown") {
                _logger.LogWarning("Incorrect DeviceType input. Only 'LS Vial' and 'CRM' is acceptable.");
                return BadRequest("Error: Device Type must be 'LS Vial' or 'CRM'.");
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);
            var job = _jobsMapper.Map<Job>(newJob);

            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();

            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddNewJob),
                new { jn = job.JobNumber, user.UserName, dateCreated = DateTime.UtcNow.ToShortDateString() });
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJob(int jobId, [FromBody]JobDTO updatedJob) {
            var job = await _context.Jobs.FindAsync(jobId);
            job = _jobsMapper.Map<Job>(updatedJob);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateJob),
                _jobsMapper.Map<JobDTO>(job));
        }

        [HttpPut("{jobId}/updatecrm/{crmId}")]
        public async Task<IActionResult> UpdateCRMOfJob(int jobId, int crmId) {
            var job = await _context.Jobs.FindAsync(jobId);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            bool crmExists = await _context.ContinuousRadonMonitors.AnyAsync(x => x.CRMId == crmId);
            if (crmExists) {
                var cRM = await _context.ContinuousRadonMonitors.FindAsync(crmId);
                cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
                job.ContinousRadonMonitor = cRM;
            } else {
                return BadRequest("Error: No CRM with that CRM Id exists.");
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(UpdateCRMOfJob), job
                );
        }

        [HttpPut("{jobId}/updatelsvial/{vialId}")]
        public async Task<IActionResult> UpdateLSVialOfJob(int jobId, int vialId) {
            var job = await _context.Jobs.FindAsync(jobId);
            var vial = await _context.LSVials.FirstOrDefaultAsync(x => x.LSVialId == vialId);
            job.LSvial = vial;
            var user = await _context.Users.FindAsync(User.Identity.Name);
            vial.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateLSVialOfJob),
                new { job });
        }

        [HttpPut("{jobId}/updateaddress/{addressId}")]
        public async Task<IActionResult> UpdateAddressOfJob(int jobId, int addressId) {
            bool addressExists = await _context.Addresses.AnyAsync(x => x.AddressId == addressId);
            var job = await _context.Jobs.FindAsync(jobId);
            if (addressExists) {
                var address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addressId);
                job.Address = address;
            } else {
                return BadRequest("No Address with that Id.");
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateAddressOfJob),
                new { job });
        }

        [HttpDelete("{jobId}")]
        public async Task<IActionResult> DeleteJob(int jobId) {
            var job = await _context.Jobs.FindAsync(jobId);
            job.LSvial = null;
            job.ContinousRadonMonitor = null;
            job.Address = null;
            await _context.SaveChangesAsync();
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllJobs));
        }

    }
}
