using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RadonTestsManager.Controllers;
using RadonTestsManager.CRMs.Models;
using RadonTestsManager.DBContext;
using RadonTestsManager.DTOs;
using RadonTestsManager.Jobs.Models;
using RadonTestsManager.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.CRMs.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class ContinuousRadonMonitorController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _cRMMapper;
        private readonly ILogger<ContinuousRadonMonitorController> _logger;

        static ContinuousRadonMonitorController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ContinuousRadonMonitor, ContinuousRadonMonitorDTO>()
                    .ForMember(dto => dto.MonitorNumber, opt => opt.MapFrom(crm => crm.MonitorNumber))
                    .ForMember(dto => dto.SerialNumber, opt => opt.MapFrom(crm => crm.SerialNumber))
                    .ForMember(dto => dto.LastCalibrationDate, opt => opt.MapFrom(crm => crm.LastCalibrationDate))
                    .ForMember(dto => dto.PurchaseDate, opt => opt.MapFrom(crm => crm.PurchaseDate))
                    .ForMember(dto => dto.LastBatteryChangeDate, opt => opt.MapFrom(crm => crm.LastBatteryChangeDate))
                    .ForMember(dto => dto.TestStart, opt => opt.MapFrom(crm => crm.TestStart))
                    .ForMember(dto => dto.TestFinish, opt => opt.MapFrom(crm => crm.TestFinish))
                    .ForMember(dto => dto.Status, opt => opt.MapFrom(crm => crm.Status));
                cfg.CreateMap<ContinuousRadonMonitorDTO, ContinuousRadonMonitor>()
                    .ForMember(dto => dto.MonitorNumber, opt => opt.MapFrom(crm => crm.MonitorNumber))
                    .ForMember(dto => dto.SerialNumber, opt => opt.MapFrom(crm => crm.SerialNumber))
                    .ForMember(dto => dto.LastCalibrationDate, opt => opt.MapFrom(crm => crm.LastCalibrationDate))
                    .ForMember(dto => dto.PurchaseDate, opt => opt.MapFrom(crm => crm.PurchaseDate))
                    .ForMember(dto => dto.LastBatteryChangeDate, opt => opt.MapFrom(crm => crm.LastBatteryChangeDate))
                    .ForMember(dto => dto.TestStart, opt => opt.MapFrom(crm => crm.TestStart))
                    .ForMember(dto => dto.TestFinish, opt => opt.MapFrom(crm => crm.TestFinish))
                    .ForMember(dto => dto.Status, opt => opt.MapFrom(crm => crm.Status));
                cfg.CreateMap<CRMMaintenanceLogEntry, CRMMaintenanceLogEntryDTO>()
                    .ForMember(l => l.EntryId, opt => opt.MapFrom(dto => dto.EntryId))
                    .ForMember(l => l.EntryDate, opt => opt.MapFrom(dto => dto.EntryDate))
                    .ForMember(l => l.MaintenanceDescription, opt => opt.MapFrom(dto => dto.MaintenanceDescription))
                    .ForMember(l => l.ActionsTaken, opt => opt.MapFrom(dto => dto.ActionsTaken));
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
            return cRMs == null ? (ActionResult<ContinuousRadonMonitorDTO[]>)NotFound() : (ActionResult<ContinuousRadonMonitorDTO[]>)Ok(_cRMMapper.Map<ContinuousRadonMonitorDTO[]>(cRMs));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContinuousRadonMonitorDTO>> GetCRMByNumber(int id) {
            var crm = await _context.ContinuousRadonMonitors
                .FirstOrDefaultAsync(c => c.CRMId == id);
            return crm == null ? (ActionResult<ContinuousRadonMonitorDTO>)NotFound() : (ActionResult<ContinuousRadonMonitorDTO>)Ok(_cRMMapper.Map<ContinuousRadonMonitorDTO>(crm));
        }

        [HttpGet("maintenance/{id}")]
        public async Task<ActionResult<CRMMaintenanceLogEntry[]>> GetCRMMaintenanceLogs(int id) {
            var logs = await _context.CRMMaintenanceLogs.Where(x => x.CRMId.CRMId == id).ToArrayAsync();
            return (ActionResult<CRMMaintenanceLogEntry[]>)Ok(logs);
        }

        [HttpGet("jobs/{id}")]
        public async Task<ActionResult<Job[]>> GetJobsOfCRM(int id) {
            var jobs = await _context.Jobs.Where(x => x.ContinousRadonMonitor.CRMId == id).ToArrayAsync();
            return (ActionResult<Job[]>)Ok(jobs);
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
            cRM.MaintenanceLog = new List<CRMMaintenanceLogEntry> { };

            await _context.ContinuousRadonMonitors.AddAsync(cRM);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddNewCRM),
                new { cRM.SerialNumber, cRM.MonitorNumber, user.UserName, dateCreated = DateTime.UtcNow.ToShortDateString() });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCRM(int id, [FromBody]ContinuousRadonMonitorDTO updatedCRM) {
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(id);
            cRM = _cRMMapper.Map<ContinuousRadonMonitor>(updatedCRM);
            cRM.CRMId = id;
            var user = await _context.Users.FindAsync(User.Identity.Name);
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateCRM),
                updatedCRM);
        }

        [HttpPut("jobs/{id}")]
        public async Task<IActionResult> AddJobtoCRM(int id, [FromBody]Job jobToAdd) {
        
        }

        [HttpPut("maintenance/{id}")]
        public async Task<IActionResult> AddMaintenanceEntrytoCRM(int id, [FromBody]CRMMaintenanceLogEntryDTO logEntry) {
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(id);
            var user = await _context.Users.FindAsync(User.Identity.Name); 
            var entry = new CRMMaintenanceLogEntry() {
                EntryDate = logEntry.EntryDate,
                MaintenanceDescription = logEntry.MaintenanceDescription,
                ActionsTaken = logEntry.ActionsTaken,
                CRMId = cRM,
                LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString()
            };
            await _context.CRMMaintenanceLogs.AddAsync(entry);

            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString() + " MaintenaceLogEntered";
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(AddMaintenanceEntrytoCRM),
                logEntry);
        }

        [HttpPut("updateaddress/{id}")]
        public async Task<IActionResult> UpdateAddressOfCRM(int id, [FromBody]Address newAddress) {
            bool addressExists = await _context.Addresses.AnyAsync(x => x == newAddress);
            var cRM = await _context.ContinuousRadonMonitors.FindAsync(id);
            Address address;
            if (addressExists) {
                address = await _context.Addresses.FirstOrDefaultAsync(x => x == newAddress);
                cRM.Location = address;
            } else {
                cRM.Location = newAddress;
                await _context.Addresses.AddAsync(newAddress);
                await _context.SaveChangesAsync();
                address = await _context.Addresses.FirstOrDefaultAsync(x => x == newAddress);
            }
            var user = await _context.Users.FindAsync(User.Identity.Name);
            cRM.LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString();
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateAddressOfCRM),
                new { cRM.SerialNumber, address.AddressId });
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) {
        }
    }
}
