using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PX1LZ1_HSZF_2024251.Application;
using PX1LZ1_HSZF_2024251.Model;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;
using System.Linq.Expressions;

namespace PX1LZ1_HSZF_2024251.Test
{
    [TestFixture]
    public class WorldServiceTests
    {
        [Test]
        public async Task AddWorldAsync_NewWorld_AddsToDatabase()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>();
            var worldsDbSetMock = new Mock<DbSet<World>>();
            dbContextMock.Setup(db => db.Worlds).Returns(worldsDbSetMock.Object);
            dbContextMock.Setup(db => db.Worlds.Any(It.IsAny<Expression<Func<World, bool>>>())).Returns(false);
            var worldService = new WorldService(dbContextMock.Object);

            // Act
            await worldService.AddWorldAsync("NewWorld", 2023);

            // Assert
            worldsDbSetMock.Verify(m => m.Add(It.Is<World>(w => w.Name == "NewWorld" && w.CurrentYear == 2023)), Times.Once);
            dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddDamageToWorldAsync_ValidWorld_AddsDamage()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>();
            var world = new World { Id = 1, Name = "TestWorld", Damages = new List<Damage>() };
            var worldsDbSetMock = new Mock<DbSet<World>>();
            worldsDbSetMock.Setup(m => m.FirstOrDefault(It.IsAny<Expression<Func<World, bool>>>())).Returns(world);
            dbContextMock.Setup(db => db.Worlds).Returns(worldsDbSetMock.Object);
            var worldService = new WorldService(dbContextMock.Object);

            // Act
            await worldService.AddDamageToWorldAsync("TestWorld", "TestDamage", 100m, "Minor", "Physical", "Local", new List<string> { "Party1" }, "Covered", "Days");

            // Assert
            Assert.That(world.Damages, Has.Count.EqualTo(1));
            Assert.That(world.Damages.First().Description, Is.EqualTo("TestDamage"));
            dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task AddDamageToWorldAsync_HighValueDamage_TriggersEvent()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>();
            var world = new World { Id = 1, Name = "TestWorld", Damages = new List<Damage>() };
            var worldsDbSetMock = new Mock<DbSet<World>>();
            worldsDbSetMock.Setup(m => m.FirstOrDefault(It.IsAny<Expression<Func<World, bool>>>())).Returns(world);
            dbContextMock.Setup(db => db.Worlds).Returns(worldsDbSetMock.Object);
            var worldService = new WorldService(dbContextMock.Object);

            DamageEventArgs capturedEventArgs = null;
            worldService.HighValueDamageAdded += (sender, e) => capturedEventArgs = e;

            // Act
            await worldService.AddDamageToWorldAsync("TestWorld", "HighValueDamage", 2000000m, "High", "Financial", "Global", new List<string> { "Party1" }, "Covered", "Months");

            // Assert
            Assert.That(capturedEventArgs, Is.Not.Null);
            Assert.That(capturedEventArgs.WorldName, Is.EqualTo("TestWorld"));
            Assert.That(capturedEventArgs.Description, Is.EqualTo("HighValueDamage"));
            Assert.That(capturedEventArgs.Cost, Is.EqualTo(2000000m));
        }
    }
}
