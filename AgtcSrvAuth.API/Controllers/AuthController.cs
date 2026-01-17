using System.Security.Claims;
using AgtcSrvAuth.Application.Dtos;
using AgtcSrvAuth.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgtcSrvAuth.API.Controllers
{
    [ApiController]
    [Route("/v1/api/auth")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _service;

        public AuthController(ILogger<AuthController> logger, IAuthService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterFarmer([FromBody] RegisterFarmerRequest request)
        {
           var token = await _service.RegisterFarmerAsync(request);

            return Ok(token);
        }

        [HttpPost("register/sensor")]
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> RegisterSensor([FromBody] RegisterSensorRequest request)
        {
            var farmerId = Guid.Parse(User.FindFirstValue(ClaimTypes.Name)!);
            var token = await _service.RegisterSensorAsync(request, farmerId);

            return Ok(token);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
        {
            var token = await _service.AuthenticateAsync(request);
            return Ok(token);
        }

    }
}
