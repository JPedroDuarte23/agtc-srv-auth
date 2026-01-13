using AgtcSrvAuth.Domain.Models;
using AgtcSrvAuth.Infrastructure.Repository;
using MongoDB.Driver;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvAuth.Test
{
    public class SensorRepositoryTests
    {
        private readonly Mock<IMongoDatabase> _databaseMock;
        private readonly Mock<IMongoCollection<Sensor>> _collectionMock;
        private readonly SensorRepository _repository;

        public SensorRepositoryTests()
        {
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<Sensor>>();
            var indexManagerMock = new Mock<IMongoIndexManager<Sensor>>();

            _collectionMock.Setup(c => c.Indexes).Returns(indexManagerMock.Object);
            _databaseMock.Setup(db => db.GetCollection<Sensor>("sensors", null)).Returns(_collectionMock.Object);

            _repository = new SensorRepository(_databaseMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallInsertOneAsync()
        {
            // Arrange
            var sensor = new Sensor();

            // Act
            await _repository.CreateAsync(sensor);

            // Assert
            _collectionMock.Verify(c => c.InsertOneAsync(sensor, null, default), Times.Once);
        }
    }
}
