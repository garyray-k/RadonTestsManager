using System;
using System.Collections.Generic;
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
    public class LSVialController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _lsVialMapper;
        private readonly ILogger<LSVialController> _logger;

        static LSVialController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<LSVial, LSVialDTO>()
                    .ForMember(dto => dto.LSVialId, opt => opt.MapFrom(lsvial => lsvial.LSVialId))
                    .ForMember(dto => dto.SerialNumber, opt => opt.MapFrom(lsvial => lsvial.SerialNumber))
                    .ForMember(dto => dto.Status, opt => opt.MapFrom(lsvial => lsvial.Status))
                    .ForMember(dto => dto.TestStart, opt => opt.MapFrom(lsvial => lsvial.TestStart))
                    .ForMember(dto => dto.TestFinish, opt => opt.MapFrom(lsvial => lsvial.TestFinish));
                cfg.CreateMap<LSVialDTO, LSVial>()
                    .ForMember(lsvial => lsvial.LSVialId, opt => opt.MapFrom(dto => dto.LSVialId))
                    .ForMember(lsvial => lsvial.SerialNumber, opt => opt.MapFrom(dto => dto.SerialNumber))
                    .ForMember(lsvial => lsvial.Status, opt => opt.MapFrom(dto => dto.Status))
                    .ForMember(lsvial => lsvial.TestStart, opt => opt.MapFrom(dto => dto.TestStart))
                    .ForMember(lsvial => lsvial.TestFinish, opt => opt.MapFrom(dto => dto.TestFinish));
            });

            _lsVialMapper = config.CreateMapper();
        }

        public LSVialController(RadonTestsManagerContext context, ILogger<LSVialController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<LSVialDTO[]>> GetAllLSVials() {
            List<LSVial> lSVials = await _context.LSVials.ToListAsync();
            return lSVials == null ? (ActionResult<LSVialDTO[]>)NotFound() : (ActionResult<LSVialDTO[]>)Ok(_lsVialMapper.Map<LSVialDTO[]>(lSVials));
        }

        [HttpGet("{vialId}")]
        public async Task<ActionResult<LSVialDTO>> GetLSVialBySerialNumber(int vialId) {
            var lsVial = await _context.LSVials.FindAsync(vialId);
            if (lsVial == null) {
                return NotFound();
            }

            return Ok(_lsVialMapper.Map<LSVialDTO>(lsVial));
        }

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
                LastUpdatedBy = user.UserName + DateTime.UtcNow.ToShortDateString()
            };

            await _context.LSVials.AddAsync(lSVial);

            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetLSVialBySerialNumber),
                new { sn = lSVial.SerialNumber, user, dateCreated = DateTime.UtcNow});
        }

        [HttpPut("{vialIs}")]
        public async Task<IActionResult> UpdateLSVialWithoutJobAddition(int vialId, [FromBody]LSVialDTO updatedLSVial) {
            var lSVial = await _context.LSVials.FindAsync(vialId);
            lSVial.LSVialId = updatedLSVial.LSVialId;
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

        [HttpDelete("{vialId}")]
        public async Task<IActionResult> DeleteLSVial(int vialId) {
            var lSVial = await _context.LSVials.FindAsync(vialId);
            _context.LSVials.Remove(lSVial);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllLSVials));
        }
    }
}
