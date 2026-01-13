using AgtcSrvAuth.Domain.Models;
using System.Numerics;

namespace AgtcSrvAuth.Application.Interfaces;

public interface IFarmerRepository
{
    Task CreateAsync(Farmer user);
    Task<Farmer?> GetByEmailAsync(string email);
    Task<Farmer?> GetByIdAsync(Guid id);
    Task<IEnumerable<Farmer>> GetAllAsync();
    Task AddAsync(Farmer user);
    Task UpdateAsync(Farmer user);
    Task DeleteAsync(Guid id);
}
