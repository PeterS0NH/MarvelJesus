using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PX1LZ1_HSZF_2024251.Application;
using PX1LZ1_HSZF_2024251.Persistence.MsSql;

namespace PX1LZ1_HSZF_2024251.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            // Szolgáltatások regisztrálása közvetlenül
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=persondb;Integrated Security=True;MultipleActiveResultSets=true"));
            services.AddTransient<JsonService>();
            services.AddTransient<ReportService>();
            services.AddTransient<WorldService>();

            var serviceProvider = services.BuildServiceProvider();

            var reportService = serviceProvider.GetRequiredService<ReportService>();
            var jsonService = serviceProvider.GetRequiredService<JsonService>();
            var worldService = serviceProvider.GetRequiredService<WorldService>();

            worldService.HighValueDamageAdded += (sender, e) =>
            {
                System.Console.WriteLine($"Értesítés: Magas értékű kár hozzáadva a(z) {e.WorldName} világban. Leírás: {e.Description}, Költség: ${e.Cost:N0}");
            };

            while (true)
            {
                System.Console.WriteLine("Válasszon egy opciót:");
                System.Console.WriteLine("1. Adatok betöltése JSON fájlból");
                System.Console.WriteLine("2. Új világ hozzáadása");
                System.Console.WriteLine("3. Kár hozzáadása egy meglévő világhoz");
                System.Console.WriteLine("4. Világ károsodási összefoglaló jelentés generálása (TXT)");
                System.Console.WriteLine("5. Javítási időtartam jelentés generálása (XML)");
                System.Console.WriteLine("6. Kategória összefoglaló jelentés generálása (TXT)");
                System.Console.WriteLine("7. Biztosítási összefoglaló jelentés generálása (TXT)");
                System.Console.WriteLine("8. Károk listázása egy adott világhoz (lapozással)");
                System.Console.WriteLine("9. Kilépés");

                var option = System.Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case "1":
                            System.Console.WriteLine("Adja meg a JSON fájl elérési útját:");
                            string filePath = System.Console.ReadLine();
                            await jsonService.ProcessJsonFileAsync(filePath);
                            System.Console.WriteLine("JSON fájl sikeresen feldolgozva.");
                            break;

                        case "2":
                            System.Console.WriteLine("Adja meg a világ nevét:");
                            string worldName = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg az aktuális évet:");
                            int year = int.Parse(System.Console.ReadLine());
                            await worldService.AddWorldAsync(worldName, year);
                            System.Console.WriteLine("Új világ sikeresen hozzáadva.");
                            break;

                        case "3":
                            System.Console.WriteLine("Adja meg a világ nevét:");
                            string existingWorld = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg a kár leírását:");
                            string description = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg a kár költségét:");
                            decimal cost = decimal.Parse(System.Console.ReadLine());
                            System.Console.WriteLine("Adja meg a kár kategóriáját (pl. minor, moderate, high):");
                            string category = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg a kár típusát (pl. physical, financial):");
                            string damageType = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg a kár hatását (pl. local, global):");
                            string impact = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg az érintett feleket (vesszővel elválasztva):");
                            List<string> involvedParties = System.Console.ReadLine().Split(',').ToList();
                            System.Console.WriteLine("Adja meg a biztosítási fedezetet (covered/uncovered):");
                            string insuranceCoverage = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg a javítási időtartamot (pl. hours, days, months):");
                            string repairDuration = System.Console.ReadLine();

                            await worldService.AddDamageToWorldAsync(existingWorld, description, cost, category, damageType, impact, involvedParties, insuranceCoverage, repairDuration);
                            System.Console.WriteLine("Kár sikeresen hozzáadva.");
                            break;

                        case "4":
                            System.Console.WriteLine("Adja meg a könyvtárat a jelentés mentéséhez:");
                            string summaryDir = System.Console.ReadLine();
                            await reportService.GenerateSummaryReportAsync(summaryDir);
                            System.Console.WriteLine("Összefoglaló jelentés sikeresen generálva.");
                            break;

                        case "5":
                            System.Console.WriteLine("Adja meg a világ nevét a javítási jelentéshez:");
                            string repairWorld = System.Console.ReadLine();
                            System.Console.WriteLine("Adja meg a könyvtárat a jelentés mentéséhez:");
                            string repairDir = System.Console.ReadLine();
                            await reportService.GenerateRepairReportAsync(repairDir, repairWorld);
                            System.Console.WriteLine("Javítási jelentés sikeresen generálva.");
                            break;

                        case "6":
                            System.Console.WriteLine("Adja meg a könyvtárat a jelentés mentéséhez:");
                            string categoryDir = System.Console.ReadLine();
                            await reportService.GenerateCategorySummaryReportAsync(categoryDir);
                            System.Console.WriteLine("Kategória összefoglaló jelentés sikeresen generálva.");
                            break;

                        case "7":
                            System.Console.WriteLine("Adja meg a könyvtárat a jelentés mentéséhez:");
                            string insuranceDir = System.Console.ReadLine();
                            await reportService.GenerateInsuranceSummaryReportAsync(insuranceDir);
                            System.Console.WriteLine("Biztosítási összefoglaló jelentés sikeresen generálva.");
                            break;

                        case "8":
                            System.Console.WriteLine("Adja meg a világ nevét:");
                            string worldForList = System.Console.ReadLine();
                            await worldService.ListDamagesWithPaginationAsync(worldForList);
                            break;

                        case "9":
                            System.Console.WriteLine("Kilépés...");
                            return;

                        default:
                            System.Console.WriteLine("Érvénytelen opció. Kérem, próbálja újra.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Hiba: {ex.Message}");
                }

                System.Console.WriteLine("\nNyomjon meg egy billentyűt a folytatáshoz...");
                System.Console.ReadKey();
                System.Console.Clear();
            }
        }
    }
}