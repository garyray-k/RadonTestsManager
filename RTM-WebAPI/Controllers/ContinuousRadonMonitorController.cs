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
using RTM.Server.Utility;

namespace RadonTestsManager.Controllers {
    //[Authorize]
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class ContinuousRadonMonitorController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _cRMMapper;
        private readonly ILogger<ContinuousRadonMonitorController> _logger;

        static ContinuousRadonMonitorController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ContinuousRadonMonitor, ContinuousRadonMonitorDTO>()
                    .ForMember(dto => dto.CrmId, opt => opt.MapFrom(crm => crm.CRMId))
                    .ForMember(dto => dto.MonitorNumber, opt => opt.MapFrom(crm => crm.MonitorNumber))
                    .ForMember(dto => dto.SerialNumber, opt => opt.MapFrom(crm => crm.SerialNumber))
                    .ForMember(dto => dto.LastCalibrationDate, opt => opt.MapFrom(crm => crm.LastCalibrationDate))
                    .ForMember(dto => dto.PurchaseDate, opt => opt.MapFrom(crm => crm.PurchaseDate))
                    .ForMember(dto => dto.LastBatteryChangeDate, opt => opt.MapFrom(crm => crm.LastBatteryChangeDate))
                    .ForMember(dto => dto.TestStart, opt => opt.MapFrom(crm => crm.TestStart))
                    .ForMember(dto => dto.TestFinish, opt => opt.MapFrom(crm => crm.TestFinish))
                    .ForMember(dto => dto.Status, opt => opt.MapFrom(crm => crm.Status))
                    .ForMember(dto => dto.AddressId, opt => opt.MapFrom(crm => crm.Address.AddressId))
                    .ForMember(dto => dto.MaintenanceLog, opt => opt.MapFrom(crm => crm.MaintenanceLogHistory.Select(x => x.EntryId)))
                    .ForMember(dto => dto.JobHistory, opt => opt.MapFrom(crm => crm.JobHistory.Select(x => x.JobId)));
                cfg.CreateMap<ContinuousRadonMonitorDTO, ContinuousRadonMonitor>()
                    .ForMember(crm => crm.CRMId, opt => opt.MapFrom(dto => dto.CrmId))
                    .ForMember(crm => crm.MonitorNumber, opt => opt.MapFrom(dto => dto.MonitorNumber))
                    .ForMember(crm => crm.SerialNumber, opt => opt.MapFrom(dto => dto.SerialNumber))
                    .ForMember(crm => crm.LastCalibrationDate, opt => opt.MapFrom(dto => dto.LastCalibrationDate))
                    .ForMember(crm => crm.PurchaseDate, opt => opt.MapFrom(dto => dto.PurchaseDate))
                    .ForMember(crm => crm.LastBatteryChangeDate, opt => opt.MapFrom(dto => dto.LastBatteryChangeDate))
                    .ForMember(crm => crm.TestStart, opt => opt.MapFrom(dto => dto.TestStart))
                    .ForMember(crm => crm.TestFinish, opt => opt.MapFrom(dto => dto.TestFinish))
                    .ForMember(crm => crm.Status, opt => opt.MapFrom(dto => dto.Status))
                    .ForMember(crm => crm.Address, opt => opt.Ignore())
                    .ForMember(crm => crm.MaintenanceLogHistory, opt => opt.Ignore())
                    .ForMember(crm => crm.JobHistory, opt => opt.Ignore());
                cfg.CreateMap<CRMMaintenanceLogEntry, CRMMaintenanceLogEntryDTO>()
                    .ForMember(l => l.EntryId, opt => opt.MapFrom(entry => entry.EntryId))
                    .ForMember(l => l.EntryDate, opt => opt.MapFrom(entry => entry.EntryDate))
                    .ForMember(l => l.MaintenanceDescription, opt => opt.MapFrom(entry => entry.MaintenanceDescription))
                    .ForMember(l => l.ActionsTaken, opt => opt.MapFrom(entry => entry.ActionsTaken));
            });
            _cRMMapper = config.CreateMapper();
        }

        public ContinuousRadonMonitorController(RadonTestsManagerContext context, ILogger<ContinuousRadonMonitorController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ContinuousRadonMonitorDTO[]>> GetAllCRMs() {
            List<ContinuousRadonMonitor> cRMs = await _context.ContinuousRadonMonitors.ToListAsync();
            if (cRMs == null) {
                return (ActionResult<ContinuousRadonMonitorDTO[]>)NotFound();
            }
            var results = _cRMMapper.Map<ContinuousRadonMonitorDTO[]>(cRMs);
            foreach (var crm in results) {
                var jobs = await _context.Jobs
                                    .Where(x => x.ContinousRadonMonitor.CRMId == crm.CrmId)
                                    .Select(x => x.JobNumber)
                                    .ToArrayAsync();
                crm.JobHistory = jobs.ToList();
            }
            return (ActionResult<ContinuousRadonMonitorDTO[]>)Ok(results);
        }

        [HttpGet("{crmId}")]
        public async Task<ActionResult<ContinuousRadonMonitorDTO>> GetCRMByNumber(int crmId) {
            var crm = await _context.ContinuousRadonMonitors
                .FirstOrDefaultAsync(c => c.CRMId == crmId);
            return crm == null ? (ActionResult<ContinuousRadonMonitorDTO>)NotFound() : (ActionResult<ContinuousRadonMonitorDTO>)Ok(_cRMMapper.Map<ContinuousRadonMonitorDTO>(crm));
        }

        [HttpGet("maintenance/{crmId}")]
        public async Task<ActionResult<CRMMaintenanceLogEntryDTO[]>> GetCRMMaintenanceLogs(int crmId) {
            var crm = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            var result = crm.MaintenanceLogHistory.ToArray();
            return (ActionResult<CRMMaintenanceLogEntryDTO[]>)Ok(result);
        }

        [HttpGet("jobs/{crmId}")]
        public async Task<ActionResult<List<JobDTO>[]>> GetJobsOfCRM(int crmId) {
            var crm = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            var result = crm.JobHistory.ToArray();
            return (ActionResult<List<JobDTO>[]>)Ok(result);
        }

        [HttpPost("")]
        public async Task<ActionResult<ContinuousRadonMonitorDTO>> AddNewCRM([FromBody]ContinuousRadonMonitorDTO newCRM) {
            if (await _context.ContinuousRadonMonitors.AnyAsync(c => c.SerialNumber == newCRM.SerialNumber)) {
                return BadRequest("Error: A CRM with that Serial Number already exists.");
            }
            if (await _context.ContinuousRadonMonitors.AnyAsync(c => c.MonitorNumber == newCRM.MonitorNumber)) {
                return BadRequest("Error: A CRM with that Monitor Number already exists.");
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);

            var cRM = _cRMMapper.Map<ContinuousRadonMonitor>(newCRM);
            cRM.TestFinish = cRM.TestStart.AddDays(2);
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();

            await _context.ContinuousRadonMonitors.AddAsync(cRM);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddNewCRM),
                new { cRM.SerialNumber, cRM.MonitorNumber, user.UserName, dateCreated = DateTime.UtcNow.ToShortDateString() });
        }

        [HttpPut("{crmId}")]
        public async Task<IActionResult> UpdateCRM(int crmId, [FromBody]ContinuousRadonMonitorDTO updatedCRM) {
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            cRM = _cRMMapper.Map<ContinuousRadonMonitor>(updatedCRM);
            cRM.CRMId = crmId;
            var user = await _context.Users.FindAsync(User.Identity.Name);
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateCRM),
                updatedCRM);
        }

        [HttpPut("{crmId}/jobs/{jobId}")]
        public async Task<IActionResult> AddJobtoCRM(int crmId, int jobIdToAdd) {
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            bool jobExists = await _context.Jobs.AnyAsync(x => x.JobId == jobIdToAdd);
            if (jobExists) {
                var job = await _context.Jobs.FindAsync(jobIdToAdd);
                job.ContinousRadonMonitor = cRM;
                job.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            } else {
                return BadRequest("Error: No Job with that Job Id exists.");
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddJobtoCRM), cRM
                );
        }

        [HttpPut("{crmId}/maintenance/{entryId}")]
        public async Task<IActionResult> AddMaintenanceEntrytoCRM(int crmId, int entryId) {
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            var user = await _context.Users.FindAsync(User.Identity.Name);
            var entry = await _context.CRMMaintenanceLogs.FindAsync(entryId);
            entry.ContinuousRadonMonitor = cRM;
            entry.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString() + " MaintenaceLogEntered";
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(AddMaintenanceEntrytoCRM),
                cRM);
        }

        [HttpPut("{crmId}/address/{addressId}")]
        public async Task<IActionResult> UpdateAddressOfCRM(int crmId, int addressId) {
            bool addressExists = await _context.Addresses.AnyAsync(x => x.AddressId == addressId);
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            var address = await _context.Addresses.FindAsync(addressId);
            if (addressExists) {
                cRM.Address = address;
            } else {
                return BadRequest("No Address with that Id found.");
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateAddressOfCRM),
                new { cRM.SerialNumber, address.AddressId });
        }

        [HttpDelete("{crmId}")]
        public async Task<IActionResult> Delete(int crmId) {
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(crmId);
            cRM.Address = null;
            cRM.MaintenanceLogHistory.Clear();
            cRM.JobHistory.Clear();
            await _context.SaveChangesAsync();
            _context.ContinuousRadonMonitors.Remove(cRM);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllCRMs));
        }
    }
}
