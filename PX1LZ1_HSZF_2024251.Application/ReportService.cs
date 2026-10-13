using Microsoft.EntityFrameworkCore;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;
using System.Text;
using System.Xml.Linq;

namespace PX1LZ1_HSZF_2024251.Application
{
    public class ReportService
    {
        private readonly AppDbContext _dbContext;

        public ReportService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GenerateSummaryReportAsync(string directoryPath)
        {
            var worldSummaries = await _dbContext.Worlds
                .Select(w => new
                {
                    WorldName = w.Name,
                    TotalDamage = w.Damages.Sum(d => d.Cost)
                })
                .ToListAsync();

            string fileName = $"Summary_{DateTime.Now:yyyyMMdd}.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var summary in worldSummaries)
                {
                    await writer.WriteLineAsync($"{summary.WorldName}: ${summary.TotalDamage:N2}");
                }
            }
        }

        public async Task GenerateRepairReportAsync(string directoryPath, string worldName)
        {
            var repairSummary = await _dbContext.Damages
                .Where(d => d.World.Name == worldName)
                .GroupBy(d => d.RepairDuration)
                .Select(g => new
                {
                    RepairDuration = g.Key,
                    IncidentCount = g.Count(),
                    TotalCost = g.Sum(d => d.Cost)
                })
                .ToListAsync();

            string fileName = $"{worldName}_RepairReport.xml";
            string filePath = Path.Combine(directoryPath, fileName);

            XDocument doc = new XDocument(
                new XElement("RepairReport",
                    new XElement("World", worldName),
                    repairSummary.Select(r =>
                        new XElement("RepairDuration",
                            new XAttribute("Duration", r.RepairDuration),
                            new XElement("IncidentCount", r.IncidentCount),
                            new XElement("TotalCost", $"${r.TotalCost:N2}")
                        )
                    )
                )
            );

            doc.Save(filePath);
        }

        public async Task GenerateCategorySummaryReportAsync(string directoryPath)
        {
            var categorySummary = await _dbContext.Damages
                .GroupBy(d => d.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    DamageCount = g.Count(),
                    TotalCost = g.Sum(d => d.Cost)
                })
                .ToListAsync();

            string fileName = "CategorySummary.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var summary in categorySummary)
                {
                    await writer.WriteLineAsync($"{summary.Category}: Események száma: {summary.DamageCount}, Össz költség: ${summary.TotalCost:N2}");
                }
            }
        }

        public async Task GenerateInsuranceSummaryReportAsync(string directoryPath)
        {
            var insuranceSummary = await _dbContext.Damages
                .GroupBy(d => d.InsuranceCoverage)
                .Select(g => new
                {
                    InsuranceCoverage = g.Key,
                    DamageCount = g.Count(),
                    TotalCost = g.Sum(d => d.Cost)
                })
                .ToListAsync();

            string fileName = "InsuranceSummary.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var summary in insuranceSummary)
                {
                    await writer.WriteLineAsync($"{summary.InsuranceCoverage}: Események száma: {summary.DamageCount}, Össz költség: ${summary.TotalCost:N2}");
                }
            }
        }
    }
}
