using System.Numerics;
using AgtcSrvAuth.Domain.Models;

namespace AgtcSrvAuth.Application.Interfaces;

public interface IPropertyRepository
{
    Task<Field?> GetFieldByIdAndOwnerAsync(Guid fieldId, Guid ownerId);

    Task<Property?> GetPropertyByFieldAndOwnerAsync(Guid fieldId, Guid ownerId);
}
