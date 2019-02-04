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

        [HttpGet("{cRMSerialNum}")]
        public async Task<ActionResult<ContinuousRadonMonitorDTO>> GetCRMByNumber(int cRMSerialNum) {
            var crm = await _context.ContinuousRadonMonitors
                .FirstOrDefaultAsync(c => c.SerialNumber == cRMSerialNum);
            return crm == null ? (ActionResult<ContinuousRadonMonitorDTO>)NotFound() : (ActionResult<ContinuousRadonMonitorDTO>)Ok(_cRMMapper.Map<ContinuousRadonMonitorDTO>(crm));
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
            var cRM = new ContinuousRadonMonitor {
                MonitorNumber = newCRM.MonitorNumber,
                SerialNumber = newCRM.SerialNumber,
                LastCalibrationDate = newCRM.LastCalibrationDate,
                PurchaseDate = newCRM.PurchaseDate,
                LastBatteryChangeDate = newCRM.LastBatteryChangeDate,
                TestStart = newCRM.TestStart,
                TestFinish = newCRM.TestStart.AddDays(2),
                Status = newCRM.Status,
                LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString()
            };

            await _context.ContinuousRadonMonitors.AddAsync(cRM);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddNewCRM),
                new { cRM.SerialNumber, cRM.MonitorNumber, user.UserName, dateCreated = DateTime.UtcNow.ToShortDateString() });
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
