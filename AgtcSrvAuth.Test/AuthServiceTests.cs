using AgtcSrvAuth.Application.Dtos;
using AgtcSrvAuth.Application.Exceptions;
using AgtcSrvAuth.Application.Interfaces;
using AgtcSrvAuth.Application.Services;
using AgtcSrvAuth.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvAuth.Test
{
    public class AuthServiceTests
    {
        private readonly Mock<IFarmerRepository> _farmerRepositoryMock;
        private readonly Mock<ISensorRepository> _sensorRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _farmerRepositoryMock = new Mock<IFarmerRepository>();
            _sensorRepositoryMock = new Mock<ISensorRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            
            var jwtSigningKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("your-super-secret-key-that-is-long-enough"));

            _authService = new AuthService(
                _farmerRepositoryMock.Object,
                _configurationMock.Object,
                _loggerMock.Object,
                jwtSigningKey,
                _sensorRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new AuthenticateRequest("test@test.com", "password");
            var farmer = new Farmer { Id = Guid.NewGuid(), Email = "test@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _farmerRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(farmer);

            // Act
            var result = await _authService.AuthenticateAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TokenResponse>(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorizedException_WhenEmailDoesNotExist()
        {
            // Arrange
            var request = new AuthenticateRequest("test@test.com", "password");
            _farmerRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.AuthenticateAsync(request));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorizedException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var request = new AuthenticateRequest("test@test.com", "wrongpassword");
            var farmer = new Farmer { Id = Guid.NewGuid(), Email = "test@test.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _farmerRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(farmer);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.AuthenticateAsync(request));
        }

        [Fact]
        public async Task RegisterFarmerAsync_ShouldReturnToken_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var request = new RegisterFarmerRequest("Test Farmer", "test@test.com", "password");
            _farmerRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((Farmer)null);

            // Act
            var result = await _authService.RegisterFarmerAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TokenResponse>(result);
            _farmerRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Farmer>()), Times.Once);
        }

        [Fact]
        public async Task RegisterFarmerAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new RegisterFarmerRequest("Test Farmer", "test@test.com", "password");
            var existingFarmer = new Farmer { Id = Guid.NewGuid(), Email = "test@test.com" };
            _farmerRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync(existingFarmer);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterFarmerAsync(request));
        }

        [Fact]
        public async Task RegisterFarmerAsync_ShouldThrowModifyDatabaseException_WhenRepositoryFails()
        {
            // Arrange
            var request = new RegisterFarmerRequest("Test Farmer", "test@test.com", "password");
            _farmerRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((Farmer)null);
            _farmerRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Farmer>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _authService.RegisterFarmerAsync(request));
        }

        [Fact]
        public async Task RegisterSensorAsync_ShouldReturnToken_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var request = new RegisterSensorRequest(AgtcSrvAuth.Domain.Enums.SensorType.Umidade, "12345", Guid.NewGuid());
            var farmer = new Farmer { Id = farmerId };
            _farmerRepositoryMock.Setup(r => r.GetByIdAsync(farmerId)).ReturnsAsync(farmer);

            // Act
            var result = await _authService.RegisterSensorAsync(request, farmerId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TokenResponse>(result);
            _sensorRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Sensor>()), Times.Once);
        }

        [Fact]
        public async Task RegisterSensorAsync_ShouldThrowUnauthorizedException_WhenFarmerDoesNotExist()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var request = new RegisterSensorRequest(AgtcSrvAuth.Domain.Enums.SensorType.Umidade, "12345", Guid.NewGuid());
            _farmerRepositoryMock.Setup(r => r.GetByIdAsync(farmerId)).ReturnsAsync((Farmer)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.RegisterSensorAsync(request, farmerId));
        }

        [Fact]
        public async Task RegisterSensorAsync_ShouldThrowModifyDatabaseException_WhenRepositoryFails()
        {
            // Arrange
            var farmerId = Guid.NewGuid();
            var request = new RegisterSensorRequest(AgtcSrvAuth.Domain.Enums.SensorType.Umidade, "12345", Guid.NewGuid());
            var farmer = new Farmer { Id = farmerId };
            _farmerRepositoryMock.Setup(r => r.GetByIdAsync(farmerId)).ReturnsAsync(farmer);
            _sensorRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Sensor>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<ModifyDatabaseException>(() => _authService.RegisterSensorAsync(request, farmerId));
        }
    }
}
