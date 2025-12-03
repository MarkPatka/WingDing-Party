using Microsoft.AspNetCore.Mvc;
using WingDing_Party.Model;
using System.Net;

namespace WingDing_Party.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private static readonly string[] Students = new[]
        {
            "Mark", "Dmitriy", "Magad", "Alexey", "Ivan"
        };

        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<ResponseServer> GetStudents()
        {
            return Ok(new ResponseServer
            {
                StatusCode = HttpStatusCode.OK,
                Result = Students
            });
        }
    }
}
