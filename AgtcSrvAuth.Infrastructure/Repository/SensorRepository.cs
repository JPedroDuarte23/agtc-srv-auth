using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Application.Interfaces;
using AgtcSrvAuth.Domain.Models;
using MongoDB.Driver;

namespace AgtcSrvAuth.Infrastructure.Repository;

public class SensorRepository : ISensorRepository
{

    private readonly IMongoCollection<Sensor> _collection;

    public SensorRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Sensor>("sensors");
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<Sensor>.IndexKeys.Ascending(u => u.Serial);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Sensor>(indexKeys, indexOptions);

        _collection.Indexes.CreateOne(indexModel);
    }
    public async Task CreateAsync(Sensor sensor)
    {
        await _collection.InsertOneAsync(sensor);
    }
}
