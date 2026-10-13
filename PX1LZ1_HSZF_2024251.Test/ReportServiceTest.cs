using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PX1LZ1_HSZF_2024251.Application;
using PX1LZ1_HSZF_2024251.Model;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;

namespace PX1LZ1_HSZF_2024251.Test
{
    [TestFixture]
    public class ReportServiceTests
    {
        [Test]
        public async Task GenerateSummaryReportAsync_CreatesFileWithCorrectContent()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>();
            var worldSummaries = new List<World>
    {
        new World
        {
            Name = "TestWorld",
            Damages = new List<Damage> { new Damage { Cost = 1000m } }
        }
    }.AsQueryable();

            var dbSetMock = new Mock<DbSet<World>>();
            dbSetMock.As<IQueryable<World>>()
                .Setup(m => m.Provider)
                .Returns(worldSummaries.Provider);
            dbSetMock.As<IQueryable<World>>()
                .Setup(m => m.Expression)
                .Returns(worldSummaries.Expression);
            dbSetMock.As<IQueryable<World>>()
                .Setup(m => m.ElementType)
                .Returns(worldSummaries.ElementType);
            dbSetMock.As<IQueryable<World>>()
                .Setup(m => m.GetEnumerator())
                .Returns(worldSummaries.GetEnumerator());

            dbContextMock.Setup(x => x.Worlds).Returns(dbSetMock.Object);
            var reportService = new ReportService(dbContextMock.Object);
            var directoryPath = Path.GetTempPath();

            // Act
            await reportService.GenerateSummaryReportAsync(directoryPath);

            // Assert
            var filePath = Directory.GetFiles(directoryPath, "Summary_*.txt").FirstOrDefault();
            Assert.That(filePath, Is.Not.Null);
            var fileContent = await File.ReadAllTextAsync(filePath);
            Assert.That(fileContent, Does.Contain("TestWorld: $1,000.00"));
            File.Delete(filePath);
        }

        [Test]
        public async Task GenerateRepairReportAsync_CreatesXmlFileWithCorrectContent()
        {
            // Arrange
            var dbContextMock = new Mock<AppDbContext>();
            var damages = new List<Damage>
        {
            new Damage { RepairDuration = "Days", Cost = 2000m, World = new World { Name = "TestWorld" } }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Damage>>();
            mockSet.As<IQueryable<Damage>>().Setup(m => m.Provider).Returns(damages.Provider);
            mockSet.As<IQueryable<Damage>>().Setup(m => m.Expression).Returns(damages.Expression);
            mockSet.As<IQueryable<Damage>>().Setup(m => m.ElementType).Returns(damages.ElementType);
            mockSet.As<IQueryable<Damage>>().Setup(m => m.GetEnumerator()).Returns(damages.GetEnumerator());

            dbContextMock.Setup(x => x.Damages).Returns(mockSet.Object);
            var reportService = new ReportService(dbContextMock.Object);
            var directoryPath = Path.GetTempPath();
            var worldName = "TestWorld";

            // Act
            await reportService.GenerateRepairReportAsync(directoryPath, worldName);

            // Assert
            var filePath = Path.Combine(directoryPath, $"{worldName}_RepairReport.xml");
            Assert.That(File.Exists(filePath), Is.True);
            var xmlContent = await File.ReadAllTextAsync(filePath);
            Assert.That(xmlContent, Does.Contain("<World>TestWorld</World>"));
            Assert.That(xmlContent, Does.Contain("<RepairDuration Duration=\"Days\">"));
            Assert.That(xmlContent, Does.Contain("<TotalCost>$2,000.00</TotalCost>"));
            File.Delete(filePath);
        }

        [Test]
        public async Task GenerateCategorySummaryReportAsync_CreatesFileWithCorrectContent()
        {
            // Arrange
            var damages = new List<Damage>
        {
            new Damage { Category = "Minor", Cost = 500m, World = new World { Name = "TestWorld" } },
            new Damage { Category = "Minor", Cost = 300m, World = new World { Name = "TestWorld" } }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Damage>>();
            mockSet.As<IQueryable<Damage>>().Setup(m => m.Provider).Returns(damages.Provider);
            mockSet.As<IQueryable<Damage>>().Setup(m => m.Expression).Returns(damages.Expression);
            mockSet.As<IQueryable<Damage>>().Setup(m => m.ElementType).Returns(damages.ElementType);
            mockSet.As<IQueryable<Damage>>().Setup(m => m.GetEnumerator()).Returns(damages.GetEnumerator());

            var dbContextMock = new Mock<AppDbContext>();
            dbContextMock.Setup(m => m.Damages).Returns(mockSet.Object);

            var reportService = new ReportService(dbContextMock.Object);
            var directoryPath = Path.GetTempPath();

            // Act
            await reportService.GenerateCategorySummaryReportAsync(directoryPath);

            // Assert
            var filePath = Path.Combine(directoryPath, "CategorySummary.txt");
            Assert.That(File.Exists(filePath), Is.True);
            var fileContent = await File.ReadAllTextAsync(filePath);
            Assert.That(fileContent, Does.Contain("Minor: Események száma: 2, Össz költség: $800.00"));
            File.Delete(filePath);
        }
    }
}