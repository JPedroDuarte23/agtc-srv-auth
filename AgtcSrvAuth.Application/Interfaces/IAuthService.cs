using AgtcSrvAuth.Application.Dtos;

namespace AgtcSrvAuth.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse> AuthenticateAsync(AuthenticateRequest request);

    Task<TokenResponse> RegisterFarmerAsync(RegisterFarmerRequest request);

    Task<TokenResponse> RegisterSensorAsync(RegisterSensorRequest request, Guid farmerId);
}
