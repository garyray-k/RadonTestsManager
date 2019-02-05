using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RadonTestsManager.DBContext;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RadonTestsManager.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class AddressController : Controller {
        //private readonly RadonTestsManagerContext _context;
        //private static readonly IMapper _jobsMapper;
        //private readonly ILogger<AddressController> _logger;


        //public IActionResult Index() {
        //    return View();
        //}
    }
}
