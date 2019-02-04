﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RadonTestsManager.CRMs.Models;
using RadonTestsManager.DBContext;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.LSVials.Models;
using RadonTestsManager.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class JobsController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _jobsMapper;
        private readonly ILogger<JobsController> _logger;

        static JobsController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Job, JobDTO>()
                    .ForMember(dto => dto.JobNumber, opt => opt.MapFrom(job => job.JobNumber))
                    .ForMember(dto => dto.ServiceType, opt => opt.MapFrom(job => job.ServiceType))
                    .ForMember(dto => dto.JobAddress, opt => opt.MapFrom(job => job.JobAddress))
                    .ForMember(dto => dto.ServiceDeadLine, opt => opt.MapFrom(job => job.ServiceDeadLine))
                    .ForMember(dto => dto.DeviceType, opt => opt.MapFrom(job => job.DeviceType))
                    .ForMember(dto => dto.AccessInfo, opt => opt.MapFrom(job => job.AccessInfo))
                    .ForMember(dto => dto.SpecialNotes, opt => opt.MapFrom(job => job.SpecialNotes))
                    .ForMember(dto => dto.Driver, opt => opt.MapFrom(job => job.Driver))
                    .ForMember(dto => dto.ArrivalTime, opt => opt.MapFrom(job => job.ArrivalTime));
                });

            _jobsMapper = config.CreateMapper();
            }

        public JobsController(RadonTestsManagerContext context, ILogger<JobsController> logger) {
            _context = context;
            _logger = logger;
        }

        // GET: api/jobs
        [HttpGet]
        public async Task<ActionResult<JobDTO[]>> GetAllJobs() {
            List<Job> jobs = await _context.Jobs.ToListAsync();
            return jobs == null ? (ActionResult<JobDTO[]>)NotFound() : (ActionResult<JobDTO[]>)Ok(_jobsMapper.Map<JobDTO[]>(jobs));

        }

        // GET api/values/5
        [HttpGet("{jobNum}")]
        public async Task<ActionResult<JobDTO>> GetJobByNumber(int jobNum) {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobNumber == jobNum);
            return job == null ? (ActionResult<JobDTO>)NotFound() : (ActionResult<JobDTO>)Ok(_jobsMapper.Map<JobDTO>(job));
        }

        // POST api/values
        [HttpPost("")]
        public async Task<ActionResult<JobDTO>> AddNewJob([FromBody] JobDTO newJob) {
            if (await _context.Jobs.AnyAsync(x => x.JobNumber.Equals(newJob.JobNumber))) {
                return BadRequest("Error: A Job already exists with that Job Number.");
            }
            if (newJob.DeviceType != "LS Vial" || newJob.DeviceType != "CRM") {
                return BadRequest("Error: Device Type must be 'LS Vial' or 'CRM'.")
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);
            var job = new Job() {
                JobNumber = newJob.JobNumber,
                ServiceType = newJob.ServiceType,
                JobAddress = newJob.JobAddress,
                ServiceDeadLine = newJob.ServiceDeadLine,
                DeviceType = newJob.DeviceType,
                AccessInfo = newJob.AccessInfo,
                SpecialNotes = newJob.SpecialNotes,
                Driver = newJob.Driver,
                ArrivalTime = newJob.ArrivalTime,
                LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString(),
            };

            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAllJobs),
                new { jn = job.JobNumber, user, dateCreated = DateTime.UtcNow });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody]JobDTO updatedJob) {
            var job = await _context.Jobs.FindAsync(id);
            job.JobNumber = updatedJob.JobNumber;
            job.ServiceType = updatedJob.ServiceType;
            job.ServiceDeadLine = updatedJob.ServiceDeadLine;
            job.DeviceType = updatedJob.DeviceType;
            job.AccessInfo = updatedJob.AccessInfo;
            job.SpecialNotes = updatedJob.SpecialNotes;
            job.Driver = updatedJob.Driver;
            job.ArrivalTime = updatedJob.ArrivalTime;
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName;

            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateJob),
                _jobsMapper.Map<JobDTO>(job));
        }

        [HttpPut("updatecrm/{id}")]
        public async Task<IActionResult> UpdateCRMOfJob(int id, [FromBody]int continuousRadonMonitorSerialNumber) {
            var job = await _context.Jobs.FindAsync(id);
            var CRMInDB = await _context.ContinuousRadonMonitors.FirstOrDefaultAsync(x => x.SerialNumber == continuousRadonMonitorSerialNumber);
            job.ContinousRadonMonitor = CRMInDB;
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(UpdateCRMOfJob),
                new { job.JobNumber, CRMInDB.SerialNumber });
        }

        [HttpPut("updatelsvial/{id}")]
        public async Task<IActionResult> UpdateLSVialOfJob(int id, [FromBody]int LSVialSerialNumber) {
            var job = await _context.Jobs.FindAsync(id);
            var VialInDB = await _context.LSVials.FirstOrDefaultAsync(x => x.SerialNumber == LSVialSerialNumber);
            job.LSvial = VialInDB;
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateLSVialOfJob),
                new { job.JobNumber, VialInDB.SerialNumber});
        }

        [HttpPut("updateaddress/{id}")]
        public async Task<IActionResult> UpdateAddressOfJob(int id, [FromBody]Address newAddress) {
            bool addressExists = await _context.Addresses.AnyAsync(x => x == newAddress);
            var job = await _context.Jobs.FindAsync(id);
            Address address;
            if (addressExists) {
                address = await _context.Addresses.FirstOrDefaultAsync(x => x == newAddress);
                job.JobAddress = address;
            } else {
                job.JobAddress = newAddress;
                await _context.Addresses.AddAsync(newAddress);
                await _context.SaveChangesAsync();
                address = await _context.Addresses.FirstOrDefaultAsync(x => x == newAddress);
            }
            address.JobHistory.Add(job);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateAddressOfJob),
                new { job.JobNumber });
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id) {
            var job = await _context.Jobs.FindAsync(id);
            job.LSvial = null;
            job.ContinousRadonMonitor = null;
            job.JobAddress = null;
            await _context.SaveChangesAsync();
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllJobs));
        }

    }
}