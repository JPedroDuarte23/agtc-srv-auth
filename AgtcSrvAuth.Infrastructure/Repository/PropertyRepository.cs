using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Application.Interfaces;
using AgtcSrvAuth.Domain.Models;
using MongoDB.Driver;

namespace AgtcSrvAuth.Infrastructure.Repository;

public class PropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection;

    public PropertyRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Property>("properties");
    }

    public async Task<Field?> GetFieldByIdAndOwnerAsync(Guid fieldId, Guid ownerId)
    {
        var filter = Builders<Property>.Filter.And(
            Builders<Property>.Filter.Eq(p => p.OwnerId, ownerId),
            Builders<Property>.Filter.ElemMatch(p => p.Fields, f => f.FieldId == fieldId)
        );

        var property = await _collection.Find(filter).FirstOrDefaultAsync();

        if (property == null) return null;

        return property.Fields.FirstOrDefault(f => f.FieldId == fieldId);
    }

    public async Task<Property?> GetPropertyByFieldAndOwnerAsync(Guid fieldId, Guid ownerId)
    {
        var filter = Builders<Property>.Filter.And(
            Builders<Property>.Filter.Eq(p => p.OwnerId, ownerId),
            Builders<Property>.Filter.ElemMatch(p => p.Fields, f => f.FieldId == fieldId)
        );

        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}
