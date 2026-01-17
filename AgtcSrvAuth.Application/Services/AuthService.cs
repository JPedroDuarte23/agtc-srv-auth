using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgtcSrvAuth.Application.Dtos;
using AgtcSrvAuth.Application.Interfaces;
using AgtcSrvAuth.Application.Exceptions;
using AgtcSrvAuth.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;
using AgtcSrvAuth.Domain.Enums;

namespace AgtcSrvAuth.Application.Services;

public class AuthService : IAuthService
{

    private readonly IFarmerRepository _farmerRepository;
    private readonly ISensorRepository _sensorRepository;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;
    private readonly string _jwtSigningKey;

    public AuthService(IFarmerRepository farmerRepository, IConfiguration config, ILogger<AuthService> logger, string jwtSigningKey, ISensorRepository sensorRepository)
    {
        _farmerRepository = farmerRepository;
        _config = config;
        _logger = logger;
        _jwtSigningKey = jwtSigningKey;
        _sensorRepository = sensorRepository;
    }

    public async Task<TokenResponse> AuthenticateAsync(AuthenticateRequest request)
    {
        _logger.LogInformation("Tentando autenticar fazendeiro com e-mail {Email}", request.Email);

        var farmer = await _farmerRepository.GetByEmailAsync(request.Email);

        if (farmer == null || !BCrypt.Net.BCrypt.Verify(request.Password, farmer.PasswordHash))
        {
            _logger.LogWarning("Falha na autenticação para o fazendeiro com e-mail {Email}", request.Email);
            throw new UnauthorizedException("E-mail ou senha inválidos");
        }

        _logger.LogInformation("Fazendeiro {Email} autenticado com sucesso", request.Email);
        return new TokenResponse(GenerateUserJwt(farmer));
    }

    public async Task<TokenResponse> RegisterFarmerAsync(RegisterFarmerRequest request) {
        _logger.LogInformation("Iniciando registro de novo fazendeiro com e-mail {Email}", request.Email);
        var fazendeiroExiste = await _farmerRepository.GetByEmailAsync(request.Email);

        if (fazendeiroExiste != null)
        {
            _logger.LogWarning("Tentativa de cadastro com e-mail já existente: {Email}", request.Email);
            throw new ConflictException("E-mail já cadastrado.");
        }

        var farmer = new Farmer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        try
        {
            await _farmerRepository.CreateAsync(farmer);

            _logger.LogInformation("Fazendeiro {Email} cadastrado com sucesso", request.Email);

            return new TokenResponse(GenerateUserJwt(farmer));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar fazendeiro com e-mail {Email}", request.Email);
            throw new ModifyDatabaseException(ex.Message);
        }
    }
    public async Task<TokenResponse> RegisterSensorAsync(RegisterSensorRequest request, Guid farmerId) {

        _logger.LogInformation("Iniciando registro de novo sensor com serial {Serial}", request.Serial);
        var farmer = await _farmerRepository.GetByIdAsync(farmerId);

        if (farmer == null)
        {
            _logger.LogWarning("Tentativa de registro de sensor com fazendeiro não existente: {userId}", farmerId);
            throw new UnauthorizedException("Fazendeiro não cadastrado na base de dados");
        }

        var sensor = new Sensor
        {
            Id = Guid.NewGuid(),
            Serial = request.Serial,
            FieldId = request.FieldId,
            OwnerId = farmerId,
            SensorType = request.SensorType,
            CreatedAt = DateTime.UtcNow
        };
        try
        {
            await _sensorRepository.CreateAsync(sensor);

            return new TokenResponse(GenerateSensorJwt(sensor));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar sensor com serial {Serial}", request.Serial);
            throw new ModifyDatabaseException(ex.Message);
        }
    }

private string GenerateUserJwt(Farmer farmer)
    {
        var keyBytes = Convert.FromBase64String(_jwtSigningKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, farmer.Id.ToString()),
            new Claim(ClaimTypes.Role, TokenRole.Farmer.ToString())
        }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private string GenerateSensorJwt(Sensor sensor)
    {
        var keyBytes = Convert.FromBase64String(_jwtSigningKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, sensor.Id.ToString()),
            new Claim("FieldId", sensor.FieldId.ToString()),
            new Claim(ClaimTypes.Role, TokenRole.Sensor.ToString()),
            new Claim("SensorType", sensor.SensorType.ToString())
        }),
            Expires = DateTime.UtcNow.AddYears(10),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
