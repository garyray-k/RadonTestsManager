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

namespace RadonTestsManager.Controllers {
    //[Authorize]
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class AddressController : Controller {
        private readonly RadonTestsManagerContext _context;
        private static readonly IMapper _addresssMapper;
        private readonly ILogger<AddressController> _logger;

        static AddressController() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Address, AddressDTO>()
                    .ForMember(dto => dto.AddressId, opt => opt.MapFrom(addr => addr.AddressId))
                    .ForMember(dto => dto.CustomerName, opt => opt.MapFrom(addr => addr.CustomerName))
                    .ForMember(dto => dto.Address1, opt => opt.MapFrom(addr => addr.Address1))
                    .ForMember(dto => dto.Address2, opt => opt.MapFrom(addr => addr.Address2))
                    .ForMember(dto => dto.City, opt => opt.MapFrom(addr => addr.City))
                    .ForMember(dto => dto.Country, opt => opt.MapFrom(addr => addr.Country))
                    .ForMember(dto => dto.PostalCode, opt => opt.MapFrom(addr => addr.PostalCode))
                    .ForMember(dto => dto.State, opt => opt.MapFrom(addr => addr.State))
                    .ForMember(dto => dto.JobHistory, opt => opt.MapFrom(addr => addr.JobHistory.Select(x => x.JobId)));
                cfg.CreateMap<AddressDTO, Address>()
                    .ForMember(addr => addr.AddressId, opt => opt.MapFrom(dto => dto.AddressId))
                    .ForMember(addr => addr.CustomerName, opt => opt.MapFrom(dto => dto.CustomerName))
                    .ForMember(addr => addr.Address1, opt => opt.MapFrom(dto => dto.Address1))
                    .ForMember(addr => addr.Address2, opt => opt.MapFrom(dto => dto.Address2))
                    .ForMember(addr => addr.City, opt => opt.MapFrom(dto => dto.City))
                    .ForMember(addr => addr.Country, opt => opt.MapFrom(dto => dto.Country))
                    .ForMember(addr => addr.PostalCode, opt => opt.MapFrom(dto => dto.PostalCode))
                    .ForMember(addr => addr.State, opt => opt.MapFrom(dto => dto.State));
            });

            _addresssMapper = config.CreateMapper();
        }

        public AddressController(RadonTestsManagerContext context, ILogger<AddressController> logger) {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<AddressDTO[]>> GetAllAddresses() {
            List<Address> addresses = await _context.Addresses.ToListAsync();
            if (addresses == null) {
                return (ActionResult<AddressDTO[]>)NotFound();
            }
            var results = _addresssMapper.Map<AddressDTO[]>(addresses);
            foreach (var address in results) {
                var jobs = await _context.Jobs.Where(x => x.Address.AddressId == address.AddressId).Select(x => x.JobNumber).ToArrayAsync();
                address.JobHistory = jobs.ToList();
            }
            return (ActionResult<AddressDTO[]>)Ok(results);
        }

        [HttpGet("{addressId}")]
        public async Task<ActionResult<AddressDTO>> GetAddressById(int addressId) {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(j => j.AddressId == addressId);
            return address == null 
                ? (ActionResult<AddressDTO>)NotFound() 
                : (ActionResult<AddressDTO>)Ok(_addresssMapper.Map<AddressDTO>(address));
        }

        [HttpPost("")]
        public async Task<ActionResult<AddressDTO>> AddNewAddress([FromBody] AddressDTO newAddress) {
            var address = _addresssMapper.Map<Address>(newAddress);

            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddNewAddress),
                new { newAddress });
        }

        [HttpPut("{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody]AddressDTO updatedAddress) {
            var oldAddress = await _context.Addresses.FindAsync(addressId);
            var newAddress = _addresssMapper.Map<AddressDTO, Address>(updatedAddress, oldAddress);
            oldAddress = newAddress;
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                nameof(UpdateAddress),
                new { updatedAddress });
        }

        [HttpPut("{addressId}/jobs/{jobIdToAdd}")]
        public async Task<IActionResult> AddJobtoAddress(int addressId, int jobIdToAdd) {
            var address = await _context.Addresses.FindAsync(addressId);
            bool jobExists = await _context.Jobs.AnyAsync(x => x.JobId == jobIdToAdd);
            if (jobExists) {
                var jobToAdd = await _context.Jobs.FindAsync(jobIdToAdd);
                jobToAdd.Address = address;
            } else {
                return BadRequest("Error: No Job with that Job Number exists.");
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(AddJobtoAddress), address);
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId) {
            var address = await _context.Addresses.FindAsync(addressId);
            address.JobHistory.Clear();
            await _context.SaveChangesAsync();
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetAllAddresses));
        }
    }
}
