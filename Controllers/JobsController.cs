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
                LastUpdatedBy = user.UserName
            };

            return newJob;
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
