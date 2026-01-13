using System.Diagnostics.CodeAnalysis;
using AgtcSrvAuth.Application.Interfaces;
using AgtcSrvAuth.Domain.Models;
using MongoDB.Driver;

namespace FiapSrvGames.Infrastructure.Repository;

[ExcludeFromCodeCoverage]
public class FarmerRepository : IFarmerRepository
{
    private readonly IMongoCollection<Farmer> _collection;

    public FarmerRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Farmer>("farmers");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<Farmer>.IndexKeys.Ascending(u => u.Email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Farmer>(indexKeys, indexOptions);

        _collection.Indexes.CreateOne(indexModel);
    }

    public async Task CreateAsync(Farmer farmer)
    {
        await _collection.InsertOneAsync(farmer);
    }
    public async Task<Farmer?> GetByEmailAsync(string email)
    {
        return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Farmer?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Farmer>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task AddAsync(Farmer farmer)
    {
        await _collection.InsertOneAsync(farmer);
    }

    public async Task UpdateAsync(Farmer farmer)
    {
        await _collection.ReplaceOneAsync(u => u.Id == farmer.Id, farmer);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(u => u.Id == id);
    }
}
