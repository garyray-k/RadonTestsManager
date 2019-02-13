using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RadonTestsManager.DBContext;
using RadonTestsManager.DTOs;
using RadonTestsManager.Jobs.Models;
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
                    .ForMember(dto => dto.ServiceDate, opt => opt.MapFrom(job => job.ServiceDate))
                    .ForMember(dto => dto.ServiceDeadLine, opt => opt.MapFrom(job => job.ServiceDeadLine))
                    .ForMember(dto => dto.DeviceType, opt => opt.MapFrom(job => job.DeviceType))
                    .ForMember(dto => dto.AccessInfo, opt => opt.MapFrom(job => job.AccessInfo))
                    .ForMember(dto => dto.SpecialNotes, opt => opt.MapFrom(job => job.SpecialNotes))
                    .ForMember(dto => dto.Driver, opt => opt.MapFrom(job => job.Driver))
                    .ForMember(dto => dto.TimeOfDay, opt => opt.MapFrom(job => job.TimeOfDay))
                    .ForMember(dto => dto.ArrivalTime, opt => opt.MapFrom(job => job.ArrivalTime))
                    .ForMember(dto => dto.Confirmed, opt => opt.MapFrom(job => job.Confirmed))
                    .ForMember(dto => dto.Completed, opt => opt.MapFrom(job => job.Completed))
                    .ForMember(dto => dto.JobAddress, opt => opt.MapFrom(job => job.JobAddress));
                cfg.CreateMap<JobDTO, Job>()
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
                    .ForMember(dto => dto.Completed, opt => opt.MapFrom(job => job.Completed))
                    .ForMember(dto => dto.JobAddress, opt => opt.MapFrom(job => job.JobAddress));
            });

            _jobsMapper = config.CreateMapper();
            }

        public JobsController(RadonTestsManagerContext context, ILogger<JobsController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<JobDTO[]>> GetAllJobs() {
            List<Job> jobs = await _context.Jobs.ToListAsync();
            return jobs == null ? (ActionResult<JobDTO[]>)NotFound() : (ActionResult<JobDTO[]>)Ok(_jobsMapper.Map<JobDTO[]>(jobs));
        }

        [HttpGet("{jobNum}")]
        public async Task<ActionResult<JobDTO>> GetJobByNumber(int jobNum) {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.JobNumber == jobNum);
            return job == null ? (ActionResult<JobDTO>)NotFound() : (ActionResult<JobDTO>)Ok(_jobsMapper.Map<JobDTO>(job));
        }

        [HttpPost("")]
        public async Task<ActionResult<JobDTO>> AddNewJob([FromBody] JobDTO newJob) {
            if (await _context.Jobs.AnyAsync(x => x.JobNumber.Equals(newJob.JobNumber))) {
                _logger.LogWarning("An existing Job was sent to AddNewJob.");
                return BadRequest("Error: A Job already exists with that Job Number.");
            }
            if (newJob.DeviceType != "LS Vial" || newJob.DeviceType != "CRM" || newJob.DeviceType != "Unkown") {
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody]JobDTO updatedJob) {
            var job = await _context.Jobs.FindAsync(id);
            job = _jobsMapper.Map<Job>(updatedJob);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
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
        public async Task<IActionResult> UpdateAddressOfJob(int id, [FromBody]AddressDTO newAddress) {
            bool addressExists = await _context.Addresses.AnyAsync(x => x.AddressId == newAddress.AddressId);
            var job = await _context.Jobs.FindAsync(id);
            Address address;
            if (addressExists) {
                address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == newAddress.AddressId);
            } else {
                address = new Address() {
                    CustomerName = newAddress.CustomerName,
                    Address1 = newAddress.Address1,
                    Address2 = newAddress.Address2,
                    City = newAddress.City,
                    Country = newAddress.Country,
                    PostalCode = newAddress.PostalCode,
                    State = newAddress.State
            };
                job.JobAddress = address;
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
                address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == newAddress.AddressId);
            }
            address.JobHistory.Add(job);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateAddressOfJob),
                new { job.JobNumber, address.AddressId });
        }

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
