using AgtcSrvAuth.API.Controllers;
using AgtcSrvAuth.Application.Dtos;
using AgtcSrvAuth.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using AgtcSrvAuth.API.Controllers;
using AgtcSrvAuth.Application.Dtos;
using AgtcSrvAuth.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvAuth.Test
{
    public class AuthControllerTests
    {
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _loggerMock = new Mock<ILogger<AuthController>>();
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_loggerMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public async Task RegisterFarmer_ShouldReturnOk()
        {
            // Arrange
            var request = new RegisterFarmerRequest("Test Farmer", "test@test.com", "password");
            var tokenResponse = new TokenResponse("token");
            _authServiceMock.Setup(s => s.RegisterFarmerAsync(request)).ReturnsAsync(tokenResponse);

            // Act
            var result = await _authController.RegisterFarmer(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tokenResponse, okResult.Value);
        }

        [Fact]
        public async Task RegisterSensor_ShouldReturnOk()
        {
            // Arrange
            var request = new RegisterSensorRequest(AgtcSrvAuth.Domain.Enums.SensorType.Umidade, "serial-123", Guid.NewGuid());
            var tokenResponse = new TokenResponse("token");
            var farmerId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, farmerId.ToString()),
            }, "mock"));

            _authController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _authServiceMock.Setup(s => s.RegisterSensorAsync(request, farmerId)).ReturnsAsync(tokenResponse);

            // Act
            var result = await _authController.RegisterSensor(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tokenResponse, okResult.Value);
        }

        [Fact]
        public async Task Authenticate_ShouldReturnOk()
        {
            // Arrange
            var request = new AuthenticateRequest("test@test.com","password");
            var tokenResponse = new TokenResponse("token");
            _authServiceMock.Setup(s => s.AuthenticateAsync(request)).ReturnsAsync(tokenResponse);

            // Act
            var result = await _authController.Authenticate(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tokenResponse, okResult.Value);
        }
    }
}
            // Assert
