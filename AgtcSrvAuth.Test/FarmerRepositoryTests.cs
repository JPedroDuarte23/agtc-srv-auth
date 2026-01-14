using AgtcSrvAuth.Domain.Models;
using AgtcSrvAuth.Infrastructure.Repository;
using MongoDB.Driver;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AgtcSrvAuth.Test
{
    public class FarmerRepositoryTests
    {
        private readonly Mock<IMongoDatabase> _databaseMock;
        private readonly Mock<IMongoCollection<Farmer>> _collectionMock;
        private readonly FarmerRepository _repository;

        public FarmerRepositoryTests()
        {
            _databaseMock = new Mock<IMongoDatabase>();
            _collectionMock = new Mock<IMongoCollection<Farmer>>();
            var indexManagerMock = new Mock<IMongoIndexManager<Farmer>>();

            _collectionMock.Setup(c => c.Indexes).Returns(indexManagerMock.Object);
            _databaseMock.Setup(db => db.GetCollection<Farmer>("farmers", null)).Returns(_collectionMock.Object);

            _repository = new FarmerRepository(_databaseMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallInsertOneAsync()
        {
            // Arrange
            var farmer = new Farmer();

            // Act
            await _repository.CreateAsync(farmer);

            // Assert
            _collectionMock.Verify(c => c.InsertOneAsync(farmer, null, default), Times.Once);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnFarmer()
        {
            // Arrange
            var email = "test@test.com";
            var farmer = new Farmer { Email = email };
            var cursorMock = new Mock<IAsyncCursor<Farmer>>();
            cursorMock.Setup(c => c.Current).Returns(new List<Farmer> { farmer });
            cursorMock.SetupSequence(c => c.MoveNext(default)).Returns(true).Returns(false);
            cursorMock.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Farmer>>(), It.IsAny<FindOptions<Farmer, Farmer>>(), default))
                         .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetByEmailAsync(email);

            // Assert
            Assert.Equal(farmer, result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFarmer()
        {
            // Arrange
            var id = System.Guid.NewGuid();
            var farmer = new Farmer { Id = id };
            var cursorMock = new Mock<IAsyncCursor<Farmer>>();
            cursorMock.Setup(c => c.Current).Returns(new List<Farmer> { farmer });
            cursorMock.SetupSequence(c => c.MoveNext(default)).Returns(true).Returns(false);
            cursorMock.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Farmer>>(), It.IsAny<FindOptions<Farmer, Farmer>>(), default))
                         .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            Assert.Equal(farmer, result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllFarmers()
        {
            // Arrange
            var farmers = new List<Farmer> { new Farmer(), new Farmer() };
            var cursorMock = new Mock<IAsyncCursor<Farmer>>();
            cursorMock.Setup(c => c.Current).Returns(farmers);
            cursorMock.SetupSequence(c => c.MoveNext(default)).Returns(true).Returns(false);
            cursorMock.SetupSequence(c => c.MoveNextAsync(default)).ReturnsAsync(true).ReturnsAsync(false);

            _collectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Farmer>>(), It.IsAny<FindOptions<Farmer, Farmer>>(), default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(farmers, result);
        }

        [Fact]
        public async Task AddAsync_ShouldCallInsertOneAsync()
        {
            // Arrange
            var farmer = new Farmer();

            // Act
            await _repository.AddAsync(farmer);

            // Assert
            _collectionMock.Verify(c => c.InsertOneAsync(farmer, null, default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallReplaceOneAsync()
        {
            // Arrange
            var farmer = new Farmer { Id = System.Guid.NewGuid() };

            // Act
            await _repository.UpdateAsync(farmer);

            // Assert
            _collectionMock.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Farmer>>(), farmer, It.IsAny<ReplaceOptions>(), default), Times.Once);
        }



        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteOneAsync()
        {
            // Arrange
            var id = System.Guid.NewGuid();

            // Act
            await _repository.DeleteAsync(id);

            // Assert
            _collectionMock.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Farmer>>(), default), Times.Once);
        }
    }
}
