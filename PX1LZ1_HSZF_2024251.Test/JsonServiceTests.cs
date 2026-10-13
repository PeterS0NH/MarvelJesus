using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PX1LZ1_HSZF_2024251.Application;
using PX1LZ1_HSZF_2024251.Model;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;

namespace PX1LZ1_HSZF_2024251.Test
{
    [TestFixture]
    public class JsonServiceTests
    {
        private Mock<AppDbContext> _dbContextMock;
        private JsonService _jsonService;

        [SetUp]
        public void Setup()
        {
            _dbContextMock = new Mock<AppDbContext>();
            _jsonService = new JsonService(_dbContextMock.Object);
        }

        [Test]
        public void ProcessJsonFileAsync_InvalidFile_ThrowsFileNotFoundException()
        {
            // Arrange
            var filePath = "non_existent_file.json";

            // Act & Assert
            Assert.ThrowsAsync<FileNotFoundException>(() => _jsonService.ProcessJsonFileAsync(filePath));
        }

        [Test]
        public async Task ProcessJsonFileAsync_ValidFile_ProcessesCorrectly()
        {
            // Arrange
            var filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, "{ \"Worlds\": [{ \"Name\": \"TestWorld\", \"CurrentYear\": 2023, \"Damages\": [] }] }");

            var worldsDbSetMock = new Mock<DbSet<World>>();
            _dbContextMock.Setup(db => db.Worlds).Returns(worldsDbSetMock.Object);

            // Act
            await _jsonService.ProcessJsonFileAsync(filePath);

            // Assert
            worldsDbSetMock.Verify(m => m.Add(It.Is<World>(w => w.Name == "TestWorld" && w.CurrentYear == 2023)), Times.Once);
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Cleanup
            File.Delete(filePath);
        }
    }
}